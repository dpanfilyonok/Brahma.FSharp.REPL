docker-build:
	docker build -t server.giraffe -f ./.devcontainer/server.giraffe/Dockerfile .

docker-run:
	docker run -d --rm --name server.giraffe -e PORT=5000 -p 80:80 server.giraffe 

docker-run-dev: 
	docker run -d --rm --name server.giraffe -e PORT=5000 -p 80:80 -v ./:/source server.giraffe 

docker-stop:
	docker stop server.giraffe

docker-bash:
	docker exec -it server.giraffe bash
