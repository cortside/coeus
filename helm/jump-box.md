# Jump box
You can connect to the busybox jump instance to debug anything in the k8's cluster
```
apiVersion: v1
kind: Pod
metadata:
  name: jump
spec:
  containers:
  - name: busybox
    image: progrium/busybox
    command:
      - sleep
      - "3600"
```
Then apply it:
```
kubectl --namespace dev apply -f ./k8s/debug_jumpbox.yaml
```
Then you can "jump" into it :)
```
kubectl --namespace dev exec -it jump sh
```