var MPGrid = {
    onEditorClick: function (event) {
        event.stopPropagation();
        return false;
    },
    toggleFieldChooser: function (event) {
        var evtEle = event.target;
        console.log("field chooser click....")
        var toolbarEle = $(evtEle).closest(".toolbar");
        var fieldChooser = toolbarEle.find(".field-chooser-container");
        if (fieldChooser.hasClass("fchooser-active")) {
            fieldChooser.removeClass("fchooser-active")
            fieldChooser.addClass("fchooser-inactive")
        }
        else {
            fieldChooser.addClass("fchooser-active")
            fieldChooser.removeClass("fchooser-inactive")
        }
    },
    init_fieldchooser: function (containerEle) {
        document.addEventListener('click', function (event) {
            if (containerEle)
            if (!$(event.target).hasClass("fieldchoosertrigger"))
                if ( event.target !== containerEle && !containerEle.contains(event.target)) {
                    $(containerEle).removeClass("fchooser-active")
                    $(containerEle).addClass("fchooser-inactive")
                }
        });
    }
}