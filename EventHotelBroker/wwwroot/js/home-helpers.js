// Home page helpers for tab switching
let homeDotNetHelper = null;

window.homeHelpers = {
    init: function (dotNetHelper) {
        homeDotNetHelper = dotNetHelper;
    },

    switchTab: function (tabName) {
        if (homeDotNetHelper) {
            homeDotNetHelper.invokeMethodAsync('SwitchTab', tabName);
        }
    }
};

// Global function for onclick handlers
window.switchTab = function (tabName) {
    window.homeHelpers.switchTab(tabName);
};
