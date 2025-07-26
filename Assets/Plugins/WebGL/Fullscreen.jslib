mergeInto(LibraryManager.library, {
  IsBrowserFullscreen: function () {
    return document.fullscreenElement !== null;
  }
});