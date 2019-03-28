var lastPlaylist = null;
var deviceID = null;
var lastSession = null;
var endOfSongPauses = 0;

$(document).ready(function () {
    
    var authenticationToken = window.sessionStorage.getItem("authenticationToken");
    var sessionId = window.sessionStorage.getItem("sessionId");
    var playlistId = GetURLParameter("playlistId");
    var username = window.sessionStorage.getItem("username");
    var userstatus = window.sessionStorage.getItem("userstatus");

    if (authenticationToken != null) {
        setupSpotifyPlayback(authenticationToken, sessionId, playlistId);
    }

    if (userstatus !== "master") {
        $("#play").hide();
        $("#progressBarDiv").hide();
    }

    $("#next").hide();
    $("#pause").hide();

    $("#export").click(function () {
        $.ajax({
            url: '/api/spotify/exportPlaylist?sessionId=' + sessionId + '&playlistId=' + playlistId,
            type: 'get',
            success: function () {
                $("#exportModal").modal('hide');
                setTimeout(function () { alert("Playlist Successfully Exported"); }, 1000);
            }
        });
    });

    $('.exportModal').click(function (e) {
        $("#exportModal").modal('show');
    });

    if (userstatus != "master") {
        $(".exportModal").hide();
    }

    /*
    if (authenticationToken != null) {
        setupSpotifyPlayback(authenticationToken, sessionId, playlistId);
    }
    */

    UpdateSessionData(sessionId, username);
    setInterval(function () {
        UpdateSessionData(sessionId, username);
    }, 1000);

    setInterval(function () {
        UpdateProgressBar();
    }, 200);

    UpdatePlaylistData(sessionId, playlistId, username);
    setInterval(function () {
        UpdatePlaylistData(sessionId, playlistId, username);
    }, 5000);

    $('#play').click(function (e) {
        e.preventDefault();
        StartOrResumePlayback(sessionId, playlistId);
    });

    $('#pause').click(function (e) {
        e.preventDefault();
        PausePlayback(sessionId, playlistId);
    });

    $('#next').click(function (e) {
        e.preventDefault();
        NextSongPlayback(sessionId, playlistId);
    });
    
    $("#search").autocomplete({
        classes: {
            "ui-autocomplete": "songs-by-artist"
        },
        source: function (request, response) {
            $.ajax({
                url: '/api/spotify/search?query=' + request.term + '&sessionId=' + sessionId,
                type: 'get',
                success: function (tracks) {
                    var trackList = tracks.map(function (track) {
                        var trackObj = new Object();
                        trackObj.label = track.name + ": " + track.artists[0].name;
                        trackObj.value = track.uri;
                        return trackObj;
                    });
                    response(trackList);
                }
            })
        },
        delay: 2000,
        select: function (event, ui) {
            var artistAndName = ui.item.label.split(": ");
            var trackName = artistAndName[0];
            var trackArtist = artistAndName[1];
            var trackUri = ui.item.value;
            $.ajax({
                url: '/api/session/' + sessionId + '/playlist/' + playlistId + '/add?trackUri=' + trackUri + '&name=' + trackName + '&artist=' + trackArtist + '&submitter=' + username,
                type: 'put',
                success: function (tracks) {
                    $("#search").val('');
                    UpdatePlaylistData(sessionId, playlistId, username);
                }
            })
        }
    });
});

function TogglePausePlay() {
    $("#pause").toggle();
    $("#play").toggle();
    $("#next").toggle();
}

function StartOrResumePlayback(sessionId, playlistId) {
    if (deviceID != null) {
        $.ajax({
            url: '/api/session/' + sessionId + "/playlist/" + playlistId + "/play?deviceId=" + deviceID,
            type: 'put',
            success: function () {
                TogglePausePlay();
            },
            error: function (e) {

            }
        });
    }
}

function PausePlayback(sessionId, playlistId) {
    if (deviceID != null) {
        $.ajax({
            url: '/api/session/' + sessionId + "/playlist/" + playlistId + "/pause",
            type: 'put',
            success: function () {
                TogglePausePlay();
            },
            error: function (e) {

            }
        });
    }
}

function NextSongPlayback(sessionId, playlistId) {
    if (deviceID != null) {
        $.ajax({
            url: '/api/session/' + sessionId + "/playlist/" + playlistId + "/next",
            type: 'put',
            success: function () {

            },
            error: function (e) {

            }
        });
    }
}

function ClearPlaylistData() {
    $("#playlistQueueTableBody tr").remove();
    $("#onboardingTableBody tr").remove();
}

function UpdatePlaylistData(sessionId, playlistId, name) {
    $.ajax({
        url: '/api/session/' + sessionId + '/playlist/' + playlistId,
        type: 'get',
        success: function (playlist) {
            if (lastPlaylist == null || JSON.stringify(lastPlaylist) !== JSON.stringify(playlist)) {
                ClearPlaylistData();
                $('#playlistNameTitle').text(playlist.name);
                var actualPlaylist = playlist.playlist;
                var onBoardingSongs = playlist.onBoardingSongs;
                for (var i in actualPlaylist) {
                    AddRowToQueueTable(actualPlaylist[i]);
                }
                for (var i in onBoardingSongs) {
                    AddRowToOnboardingTable(sessionId, playlistId, onBoardingSongs[i], name);
                }

                lastPlaylist = playlist;
            }
        },
        error: function () {
            alert("Session no longer available, redirecting");
            setTimeout(function () { exitSession() }, 3000);
        }
    });
}


function UpdateSessionData(sessionId, username) {
    $.ajax({
        url: '/api/session/' + sessionId,
        type: 'get',
        success: function (session) {
            if (session != null && (lastSession == null || JSON.stringify(lastSession) !== JSON.stringify(session))) {
                ClearSessionData();
                var users = session.users;
                if (users != null) {
                    for (var i in users) {
                        AddRowToUsersTable(users[i]);
                        if (users[i].name == username) {
                            if (users[i].readyForFreePick) {
                                $("#freePickIndicator").show();
                            } else {
                                $("#freePickIndicator").hide();
                            }
                        }
                    }
                }

                lastSession = session;
            }
        }
    });
}


function AddRowToUsersTable(user) {
    $("#userTableBody").append("<tr class='userTableRow'><td>" + user.name + "</td><td>" + user.totalSongsSelected + "</td></tr>");
}

function ClearSessionData() {
    $("#userTableBody tr").remove();
}


function AddRowToQueueTable(track) {
    var row = "<tr><td>" + track.name + "</td><td>" + track.artist + "</td></tr>";
    $("#playlistQueueTableBody").append(row);
}

function AddRowToOnboardingTable(sessionId, playlistId, onboardingSong, name) {
    var row = $('<tr />');
    row.append('<td>' + onboardingSong.song.name + '</td><td>' + onboardingSong.song.artist + '</td>');
    //var row = "<tr><td>" + onboardingSong.name + "</td><td>" + onboardingSong.artist + "</td><td>" + findVote(name, onboardingSong.votes) + "</td></tr> ";
    var starColumn = $('<td />');
    var stars = createStars(sessionId, playlistId, name, onboardingSong, findVote(name, onboardingSong.votes));
    starColumn.append(stars);
    row.append(starColumn);
    $("#onboardingTableBody").append(row);
}


function createStars(sessionId, playlistId, name, onboardingSong, rating) {
        var stars = [];
        for (i = 0; i < 5; i++) {
            var star = $('<span />').addClass('fa');
            star.addClass('fa-star');
            star.addClass(i.toString());
            if (i <= rating) {
                star.addClass('checked');
            }
            star.click(function () {
                for (i = 0; i < 5; i++) {
                    stars[i].removeClass('checked');
                }
                for (i = 0; i < 5; i++) {
                    stars[i].addClass('checked');
                    if ($(this).hasClass(i)) {
                        $.ajax({
                            url: '/api/session/' + sessionId + '/playlist/' + playlistId + '/update?trackUri=' + onboardingSong.song.trackUri + '&rating=' + i + '&username=' + name,
                            type: 'put',
                            success: function () {
                            }
                        })
                        break;
                    }
                }
            });
            stars.push(star);
    }

    return stars;
}

function findVote(name, votes) {
    
    for (var i in votes) {
        if (votes[i].user.name == name) {
            return votes[i].score;
        }
    }
    
    return -1;
}

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

function setupSpotifyPlayback(authToken, sessionId, playlistId) {

    window.onSpotifyWebPlaybackSDKReady = () => {
        const token = authToken;
        const player = new Spotify.Player({
            name: 'Web Playback SDK Quick Start Player',
            getOAuthToken: cb => { cb(token); }
        });

        // Error handling
        player.addListener('initialization_error', ({ message }) => { console.error(message); });
        player.addListener('authentication_error', ({ message }) => { console.error(message); });
        player.addListener('account_error', ({ message }) => { console.error(message); });
        player.addListener('playback_error', ({ message }) => { console.error(message); });

        // Playback status updates
        player.addListener('player_state_changed', state => {
            console.log(state);
            UpdatePlayback(state);
        });

        // Ready
        player.addListener('ready', ({ device_id }) => {
            deviceID = device_id;
            console.log('Ready with Device ID', device_id);
        });

        // Not Ready
        player.addListener('not_ready', ({ device_id }) => {
            console.log('Device ID has gone offline', device_id);
        });
        
        // Connect to the player!
        player.connect();
    };

    
}

function UpdatePlayback(state) {
    if (state) {
        UpdateProgressBarState(state);
    }
}

var progress = 0;
var playing = false;
var progressIncrement = 0;
function UpdateProgressBarState(state) {
    // every .2 seconds will increment
    progressIncrement = 100 / ((state.duration / 1000) * 5);
    playing = !state.paused;
    progress = state.position * 100 / state.duration;
}

function UpdateProgressBar() {
    if (playing) {
        progress += progressIncrement;
    }
    $("#progressBar").css('width', progress + '%');
}

function exitSession() {
    window.sessionStorage.removeItem("username");
    window.sessionStorage.removeItem("userstatus");
    window.sessionStorage.removeItem("sessionId");
    window.sessionStorage.removeItem("keep-alive");
    window.sessionStorage.removeItem("authentication-token");
    window.location.assign("/home");
}


function setupSpotifyPlayback(authToken) {
    window.onSpotifyWebPlaybackSDKReady = () => {
        const token = authToken;
        const player = new Spotify.Player({
            name: 'Web Playback SDK Quick Start Player',
            getOAuthToken: cb => { cb(token); }
        });

        // Error handling
        player.addListener('initialization_error', ({ message }) => { console.error(message); });
        player.addListener('authentication_error', ({ message }) => { console.error(message); });
        player.addListener('account_error', ({ message }) => { console.error(message); });
        player.addListener('playback_error', ({ message }) => { console.error(message); });

        // Playback status updates
        player.addListener('player_state_changed', state => {
            console.log(state);
            UpdatePlayback(state);
        });

        // Ready
        player.addListener('ready', ({ device_id }) => {
            deviceID = device_id;
            console.log('Ready with Device ID', device_id);
        });

        // Not Ready
        player.addListener('not_ready', ({ device_id }) => {
            console.log('Device ID has gone offline', device_id);
        });

        // Connect to the player!
        player.connect();
    };
}