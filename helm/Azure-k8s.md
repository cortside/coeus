# Azure Kubernetes

## HowTo: Create the azure cluster

```sh
az aks create --name yourdomain-aks \
              --resource-group kubernetes \
              --location "Central US" \
              --dns-name-prefix "yourdomain-aks" \
              --enable-aad \
              --aad-admin-group-object-ids $yourdomainAdminsGroupId \
              --attach-acr yourdomain \
              --node-count 1 \
              --node-vm-size Standard_D2s_v4 \
              --nodepool-name tempnodepool \
              --kubernetes-version 1.27 \
              --no-ssh-key \
              --enable-managed-identity
```

### Line by line:

These are the important, non-obvious parts of the command above:

- `--dns-name-prefix "yourdomain-aks"` - create the cluster with the DNS name `aks-yourdomain.centralus.cloudapp.azure.com`
- `--aad-admin-group-object-ids $yourdomainAdminsGroupId` - add the `yourdomain Admins` group as an admin of the cluster
- `--attach-acr yourdomain` - attach the `yourdomain` container registry to the cluster, **requires ability to create role assignments**
- `--enable-managed-identity` - enable managed identity for the cluster (required for Azure Policy integration, used for keyvault secrets)

## Setup Node Pools

```sh
# Dedicated system nodepool
# the taints below prevent this nodepool from being used for workloads unless they have the matching tolerations
az aks nodepool add --resource-group kubernetes --cluster-name yourdomain-aks \
    --name systempool \
    --node-vm-size Standard_DS2_v2 \
    --node-count 1 \
    --node-taints CriticalAddonsOnly=true:NoSchedule \
    --mode System

# main nodepool
# NOTE: when adding these, I needed to request a slightly higher quota for total regional vCPUs and Standard Family DSv4 vCPUs
az aks nodepool add --resource-group kubernetes --cluster-name yourdomain-aks \
    --name d4sv4pool \
    --node-vm-size Standard_D4s_v4 \
    --labels vCPU=4 ram=16 \
    --enable-cluster-autoscaler \
    --min-count 2 \
    --max-count 10

# delete the temp nodepool from the create command
az aks nodepool delete --resource-group kubernetes --cluster-name yourdomain-aks --name tempnodepool
```

## Setup static IP

```sh
CLUSTER_RESOURCE_GROUP=$(az aks show --resource-group kubernetes --name yourdomain-aks --query nodeResourceGroup -o tsv)
az network public-ip create --resource-group "$CLUSTER_RESOURCE_GROUP" \
                            --name yourdomain-aks-ip  \
                            --sku Standard  \
                            --allocation-method static \
                            --query '[publicIp.ipAddress, publicUp.dnsSettings.fqdn]'

# Get the static IP
az network public-ip show -g "$(az aks show --resource-group kubernetes --name yourdomain-aks --query nodeResourceGroup -o tsv)" -n yourdomain-aks-ip -o json | jq -r .ipAddress
```

## Kubectl configuration

### Initial setup:

it's helpful if you have kubectl installed and configured to use the cluster before you run these commands:

```sh
# add ~/.local/bin to your PATH if it's not already there
echo 'export PATH="$HOME/.local/bin:$PATH"' >> "$SHELLRC"

# install kubectl and kubelogin, configure
az login
az aks install-cli --install-location ~/.local/bin/kubectl --kubelogin-install-location ~/.local/bin/kubelogin
az aks get-credentials --admin --name yourdomain-aks -g kubernetes
kubectl config use-context yourdomain-aks
```

## Cluster role bindings

```sh
kubectl apply -f roles/roles.yaml
kubectl apply -f roles/yourdomain-admins.yaml
kubectl apply -f roles/yourdomain-engineers.yaml
kubectl apply -f roles/kubelet-api-admin.yaml
```

## Create namespaces

- `dev` - for development workloads
- `prod` - for production workloads
- `e2e` - for end-to-end testing workloads
- `system` - for system workloads

```sh
kubectl create namespace dev
kubectl create namespace prod
kubectl create namespace e2e
kubectl create namespace system
```