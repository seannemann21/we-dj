$(document).ready(function () {
    if (window.sessionStorage.getItem("sessionId") != null && window.sessionStorage.getItem("userstatus") != null && window.sessionStorage.getItem("username") != null) {
        window.location.assign("/session.html");
    }
    var sessionId = window.sessionStorage.getItem("sessionId");
    if (sessionId == null) {
    var code = GetURLParameter("code");
    if (code != null) {
        $.ajax({
            url: '/api/session?code=' + code,
            type: 'post',
            success: function (session) {
                if (session != null) {
                    window.sessionStorage.setItem("sessionId", session.id);
                    window.sessionStorage.setItem("userstatus", "master");
                    window.sessionStorage.setItem("keep-alive", session.keepAliveToken);
                    window.sessionStorage.setItem("authenticationToken", session.player.credentials.accessToken);
                    sessionId = session.id;
                }
            }
        });
        }
    }


    $('#submitUserForm').submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: '/api/session/createuser?username=' + $("#username").val() + '&sessionId=' + sessionId,
            type: 'put',
            success: function (data) {
                window.sessionStorage.setItem("username", $("#username").val());
                window.location.assign("/session.html");
            },
            error: function (e) {
                $("#userError").show();
            }
        });
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