IMAGE_NAME := server.giraffe
CONTAINER_NAME := server.giraffe

docker-build:
	docker build -t $(IMAGE_NAME) -f ./.devcontainer/server-giraffe/Dockerfile .

docker-run:
	docker run -d --rm --name $(CONTAINER_NAME) -e PORT=80 -p 8080:80 $(IMAGE_NAME)

docker-run-dev: 
	docker run -d --rm --name $(CONTAINER_NAME) -e PORT=80 -p 8080:80 -v ./:/source $(IMAGE_NAME) 

docker-stop:
	docker stop $(CONTAINER_NAME)

docker-bash:
	docker exec -it $(CONTAINER_NAME) bash
