
 $(function(){
    if(haMediaAccess()){ 
        const constraints = {video:true, audio:true};
        const video = document.getElementById("stream1");
        navigator.mediaDevices.getUserMedia(constraints).then((stream) => {video.srcObject = stream;});
    }  
});

function haMediaAccess(){
    return !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia);
}