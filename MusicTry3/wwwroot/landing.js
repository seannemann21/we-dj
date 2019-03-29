let REDIRECT_URI = null;
$.get("api/session/redirect-uri", function (data) {
    REDIRECT_URI = data;
});

$(document).ready(function () {
    if (window.sessionStorage.getItem("sessionId") != null && window.sessionStorage.getItem("userstatus") != null && window.sessionStorage.getItem("username") != null) {
        window.location.assign("/session.html");
    } else {
        window.sessionStorage.removeItem("username");
        window.sessionStorage.removeItem("userstatus");
        window.sessionStorage.removeItem("sessionId");
        window.sessionStorage.removeItem("keep-alive");
    }

    $('#joinRoom').submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: '/api/session/' + $('#session').val(),
            type: 'get',
            success: function () {
                window.sessionStorage.setItem("sessionId", $('#session').val());
                window.sessionStorage.setItem("userstatus", "regular");
                window.location.assign("/createuser.html");
            },
            error: function () {
                $('#roomError').show();
            }
        });
    });

    $('#newRoom').click(function (e) {
        window.location.assign("https://accounts.spotify.com/authorize/?response_type=code&client_id=fffa7e259c734e9d9b681b1fbf07f2f9&scope=user-read-playback-state%20streaming%20user-read-birthdate%20user-read-email%20user-read-private%20playlist-modify-public%20user-modify-playback-state&redirect_uri=" + REDIRECT_URI);
    });
});

function GetURLParameter(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return decodeURIComponent(sParameterName[1]);
        }
    }
}