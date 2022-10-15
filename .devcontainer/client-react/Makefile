IMAGE_NAME := client-react
CONTAINER_NAME := client-react

docker-build:
	docker build -t $(IMAGE_NAME) -f ./.devcontainer/client-react/Dockerfile .

docker-run:
	docker run -d --rm --name $(CONTAINER_NAME) -e PORT=5000 -p 80:80 $(IMAGE_NAME)

docker-run-dev: 
	docker run -d --rm --name $(CONTAINER_NAME) -e PORT=5000 -p 80:80 -v ./:/source $(IMAGE_NAME) 

docker-stop:
	docker stop $(CONTAINER_NAME)

docker-bash:
	docker exec -it $(CONTAINER_NAME) bash
