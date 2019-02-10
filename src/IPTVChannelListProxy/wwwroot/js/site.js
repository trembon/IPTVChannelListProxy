$(function () {
    var playlistChannelID = 0;
    $('.set-channel-btn').click(function () {
        playlistChannelID = $(this).data('id');
    });
    $('#setChannelModal form').submit(function (e) {
        e.preventDefault();
        $('#setChannelModal .search').click();
    });
    $('#setChannelModal .search').click(function (e) {
        e.preventDefault();

        $.post("/api/channel/search", { q: $("#setChannelModal .form-control").val() }, function (data) {
            var $table = $("#setChannelModal table tbody");
            $table.html("");
            $.each(data, function (i, val) {
                $table.append('<tr><td><a href="#" data-id="' + val.id + '">' + val.name + '</a></td></tr>');
            });
        });
    });
    $('#setChannelModal table tbody').on('click', 'a', function (e) {
        e.preventDefault();
        var channelText = $(this).text();

        $.post('/api/channel/setchannel', { playlistChannel: playlistChannelID, channel: $(this).data('id') }, function (data) {
            if (data) {
                $('.set-channel-btn[data-id="' + playlistChannelID + '"]').parent().find('span').text(channelText);
                $('#setChannelModal').modal('hide');
            } else {
                alert('Something went wrong, try again.');
            }
        });
    });
});