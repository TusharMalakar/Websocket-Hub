# Docker CMD
1. create an image from cmd
- docker build -t signarlapi:latest .

2. confirm the build images
- docker images
3. find all images tagname starts with "signalr"
    - docker images "signalr*" 

4. run the image ("PORT-FORWORDING")
- docker run -p 8080:80 signalrapi

5. check if the image is running or not
- docker ps

6. stop the image (two steps):
    - docker ps (to get the running image id)
    - docker stop <imges-id>

7. find all containerIds by tagname, stop and kill 
    - docker stop $(docker ps -a -q  --filter ancestor=signalrapi)
