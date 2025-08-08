import os
from typing import Optional
from opentelemetry import trace, metrics
from opentelemetry.sdk.resources import Resource
from opentelemetry.sdk.trace import TracerProvider
from opentelemetry.sdk.trace.export import BatchSpanProcessor, ConsoleSpanExporter
from opentelemetry.exporter.otlp.proto.http.trace_exporter import OTLPSpanExporter
from opentelemetry.sdk.metrics import MeterProvider
from opentelemetry.sdk.metrics.export import PeriodicExportingMetricReader
from opentelemetry.exporter.otlp.proto.http.metric_exporter import OTLPMetricExporter
from opentelemetry.sdk._logs import LoggerProvider, LoggingHandler
from opentelemetry.sdk._logs.export import BatchLogRecordProcessor, ConsoleLogExporter
from opentelemetry.exporter.otlp.proto.http._log_exporter import OTLPLogExporter
from opentelemetry.instrumentation.fastapi import FastAPIInstrumentor
from opentelemetry.instrumentation.httpx import HTTPXClientInstrumentor
# Single shared logger provider (created early when logging configured)
_logger_provider: LoggerProvider | None = None
_def_level = 0  # logging.NOTSET fallback without importing logging here

def _resource():
    return Resource.create({"service.name": os.getenv("OTEL_SERVICE_NAME", "acme-shoppingcart-mcp")})

def get_logging_handler(level: Optional[int] = None):
    """Return an OTEL LoggingHandler. Safe to call before setup_tracing.
    We lazily create a LoggerProvider so early logging still works; processors/exporters added later in setup_tracing.
    """
    global _logger_provider
    if _logger_provider is None:
        _logger_provider = LoggerProvider(resource=_resource())
    return LoggingHandler(level=level if level is not None else _def_level, logger_provider=_logger_provider)

def setup_tracing(app=None) -> None:
    # Traces
    tp = TracerProvider(resource=_resource())
    trace.set_tracer_provider(tp)
    te = os.getenv("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT") or os.getenv("OTEL_EXPORTER_OTLP_ENDPOINT")
    tp.add_span_processor(BatchSpanProcessor(OTLPSpanExporter(endpoint=te) if te else ConsoleSpanExporter()))

    # Metrics
    me = os.getenv("OTEL_EXPORTER_OTLP_METRICS_ENDPOINT") or os.getenv("OTEL_EXPORTER_OTLP_ENDPOINT")
    readers = []
    if me:
        readers.append(PeriodicExportingMetricReader(OTLPMetricExporter(endpoint=me)))
    metrics.set_meter_provider(MeterProvider(resource=_resource(), metric_readers=readers))

    # Logs - reuse existing provider if created early
    global _logger_provider
    if _logger_provider is None:
        _logger_provider = LoggerProvider(resource=_resource())
    lp = _logger_provider
    le = os.getenv("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT") or os.getenv("OTEL_EXPORTER_OTLP_ENDPOINT")
    # Avoid adding duplicate processors if setup_tracing called multiple times
    if not getattr(lp, "_mcp_processors_added", False):
        lp.add_log_record_processor(BatchLogRecordProcessor(OTLPLogExporter(endpoint=le) if le else ConsoleLogExporter()))
        setattr(lp, "_mcp_processors_added", True)

    # Instrumentation
    HTTPXClientInstrumentor().instrument()
    if app:
        FastAPIInstrumentor.instrument_app(app, excluded_urls="/metrics")


def get_tracer(name: str = "acme.shoppingcart.mcp"):
    return trace.get_tracer(name)
