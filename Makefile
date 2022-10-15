# Run

client-run:
	npm start
	
func-run:
	cd src/Server.AzureFunctions && func start

swa-run:
	swa start http://localhost:3000 --api-location http://localhost:7071

# Docker

# TODO fix
docker-rmi:
	docker rmi $$(docker images -f “dangling=true” -q)

server-giraffe:
	make -f ./.devcontainer/server-giraffe/Makefile $(cmd)

client-react:
	make -f ./.devcontainer/client-react/Makefile $(cmd)
