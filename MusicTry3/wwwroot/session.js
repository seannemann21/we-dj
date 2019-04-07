$(window).on("load", function () {
    var sessionId = window.sessionStorage.getItem("sessionId");
    var username = window.sessionStorage.getItem("username");
    var userstatus = window.sessionStorage.getItem("userstatus");

    setupPage(sessionId, username, userstatus);
    

    $('#createPlaylistButton').click(function (e) {
        e.preventDefault();
        const name = { name: $('#name').val() };
        $.ajax({
            url: '/api/session/' + sessionId + "/playlist/",
            type: 'post',
            data: JSON.stringify(name),
            contentType: 'application/json',
            success: function (data) {
                window.location.assign("/playlist.html?playlistId=" + data.id);
            },
            error: function (e) {

            }
        });
    });

    $('.exit').click(function (e) {
        e.preventDefault();
        if (userstatus == "master") {
            $("#endSessionModal").modal('show');
        } else {
            exitSession();
        }
    });

    $('#endSession').click(function (e) {
        e.preventDefault();
        exitSession();
    });
});

function exitSession() {
    window.sessionStorage.removeItem("username");
    window.sessionStorage.removeItem("userstatus");
    window.sessionStorage.removeItem("sessionId");
    window.sessionStorage.removeItem("keep-alive");
    window.sessionStorage.removeItem("authentication-token");
    window.location.assign("/");
}

function setupPage(sessionId, username, userstatus) {
    $("#sessionName").text(sessionId);

    if (userstatus !== "master") {
        $("#name").prop('disabled', true);
        $("#createPlaylistButton").prop('disabled', true);
    }

    $.ajax({
        url: '/api/session/' + sessionId + "/playlist/",
        type: 'get',
        success: function (playlists) {
            for (var i in playlists) {
                $('#playlistTableBody').append(createRowForPlaylistTable(playlists[i], sessionId, username));
            }
        },
        error: function () {
            alert("Session no longer available, redirecting");
            setTimeout(function () { exitSession() }, 1000);
        }
    });
}

function createRowForPlaylistTable(playlist, sessionId, username) {
    var row = $('<tr style="cursor: pointer;" />');
    row.append('<td>' + playlist.name + '</td>');
    row.click(function () {
        $.ajax({
            url: '/api/session/' + sessionId + "/playlist/" + playlist.id + "/load",
            type: 'put',
            success: function () {
                window.location.assign('/playlist.html?playlistId=' + playlist.id);
            }
        })
    });
    return row;
}