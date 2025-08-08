import time, os
from typing import Dict, Tuple
class CircuitOpenError(Exception): pass
class CircuitBreaker:
    def __init__(self, failure_threshold:int, recovery_seconds:float, half_open_successes:int):
        self.failure_threshold=failure_threshold; self.recovery_seconds=recovery_seconds; self.half_open_successes=half_open_successes
        self.state="closed"; self.failure_count=0; self.success_count=0; self.opened_at=0.0
    def allow_request(self):
        if self.state=="open":
            if time.time()-self.opened_at>=self.recovery_seconds: self.state="half_open"; self.success_count=0; return
            raise CircuitOpenError("circuit_open")
    def record_success(self):
        if self.state=="half_open":
            self.success_count+=1
            if self.success_count>=self.half_open_successes: self._close()
        else: self.failure_count=0
    def record_failure(self):
        if self.state=="half_open": self._open(); return
        self.failure_count+=1
        if self.failure_count>=self.failure_threshold: self._open()
    def _open(self): self.state="open"; self.opened_at=time.time()
    def _close(self): self.state="closed"; self.failure_count=0; self.success_count=0
class CircuitRegistry:
    def __init__(self):
        self.failure_threshold=int(os.getenv("CB_FAILURE_THRESHOLD","5")); self.recovery_seconds=float(os.getenv("CB_RECOVERY_SECONDS","30")); self.half_open_successes=int(os.getenv("CB_HALFOPEN_SUCCESSES","1"))
        self.scope=os.getenv("CB_SCOPE","per_route"); self._global_cb=CircuitBreaker(self.failure_threshold,self.recovery_seconds,self.half_open_successes); self._per_route:Dict[Tuple[str,str],CircuitBreaker]={}
    def get(self, method:str, path:str)->CircuitBreaker:
        if self.scope=="global": return self._global_cb
        key=(method.upper(),path)
        if key not in self._per_route: self._per_route[key]=CircuitBreaker(self.failure_threshold,self.recovery_seconds,self.half_open_successes)
        return self._per_route[key]
registry=CircuitRegistry()
