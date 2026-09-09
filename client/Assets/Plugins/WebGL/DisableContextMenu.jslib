mergeInto(LibraryManager.library, {
    DisableContextMenu: function () {
        var canvas = document.querySelector("#unity-canvas") || document.querySelector("canvas");

        if (canvas) {
            canvas.addEventListener("contextmenu", function (e) {
                e.preventDefault();
            });
        }
    }
});
