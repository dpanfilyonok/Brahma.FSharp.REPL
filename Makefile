# Run

client-run:
	npm start
	
func-run:
	cd src/Server.AzureFunctions && func start

swa-run:
	swa start http://localhost:3000 --api-location http://localhost:7071

# Docker

docker-rmi:
	docker rmi $(docker images -f “dangling=true” -q)

giraffe:
	make -f ./.devcontainer/server-giraffe/Makefile $(cmd)

react:
	make -f ./.devcontainer/client-react/Makefile $(cmd)
