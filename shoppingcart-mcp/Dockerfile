FROM python:3.11-slim
WORKDIR /app
COPY . /app
RUN pip install --no-cache-dir -r requirements.txt
ENV LOG_LEVEL=INFO SHOPPINGCART_RETRIES=3 SHOPPINGCART_TIMEOUT_SECONDS=15
EXPOSE 8000
CMD ["uvicorn","main:app","--host","0.0.0.0","--port","8000"]
