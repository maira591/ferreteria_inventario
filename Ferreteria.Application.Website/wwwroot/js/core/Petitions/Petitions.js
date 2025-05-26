var Petitions = (function () {
    function Petitions() {

    }

    Petitions.Get = function (url, callback) {
        $.get(url, function (response) { callback(response); });
    };

    return Petitions;
})();