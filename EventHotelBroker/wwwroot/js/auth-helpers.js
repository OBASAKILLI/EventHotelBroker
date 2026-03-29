// Auth helper functions for Blazor interop
window.authHelpers = {
    getCookie: function (name) {
        var match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
        return match ? decodeURIComponent(match[2]) : '';
    },
    setCookie: function (name, value, maxAgeSeconds) {
        document.cookie = name + '=' + encodeURIComponent(value) + ';path=/;max-age=' + maxAgeSeconds + ';SameSite=Strict';
    },
    deleteCookie: function (name) {
        document.cookie = name + '=;path=/;max-age=0;SameSite=Strict';
    }
};
