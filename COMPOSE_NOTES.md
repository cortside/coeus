ToDo:

- [ ] rename user from user to app
- [x] dotnet-runtime startup.sh with optional argument to skip running the service
	* to be used with bootstrap db container to make sure that files are copied to final destination -- needed for making sure sql scripts are in sql directory for when updating database
- [x] if environment == docker host then use DOCKERHOST settings directory and replace $DOCKER_HOST with actual host value
	* that way docker host name does not matter and there are fewer configurations
- [ ] nginx runtime image should have site run from /app so that it is consistent with the api images 

ToDone:
