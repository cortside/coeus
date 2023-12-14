# allow non-root docker access
* after installing docker on ubuntu host, run following so that non-root user can run docker commands:
sudo groupadd docker
sudo usermod -aG docker $USER
sudo chown root:docker /var/run/docker.sock
sudo chown -R root:docker /var/run/docker

# to allow remote docker api
* Open the file /lib/systemd/system/docker.service
* Modify the following line:
#ExecStart=/usr/bin/docker daemon -H fd:// -H tcp://0.0.0.0:2375
ExecStart=/usr/bin/dockerd -H fd:// -H tcp://0.0.0.0:2375 --containerd=/run/containerd/containerd.sock

* Reload the configuration and restart the Docker daemon:

sudo systemctl daemon-reload
sudo systemctl restart docker.service

mkdir /srv/mssql
chgrp -R 0 /srv/mssql
chmod -R g=u /srv/mssql
chown 10001:0 /srv/mssql

# Underlying phyiscal disk has unsupported sector size:
There have been 256 misaligned log IOs which required falling back to synchronous IO.  The current IO is on file /var/opt/mssql/data/master.mdf.

start sqlserver with trace flag 1800 on (to force to 4K sectors for logs)



https://docs.docker.com/engine/install/ubuntu/

```bash
for pkg in docker.io docker-doc docker-compose docker-compose-v2 podman-docker containerd runc; do sudo apt-get remove $pkg; done

# Add Docker's official GPG key:
sudo apt-get update
sudo apt-get install ca-certificates curl gnupg
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg

# Add the repository to Apt sources:
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update

sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```
