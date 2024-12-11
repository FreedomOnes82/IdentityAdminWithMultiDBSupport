var MPTabCtrl = (function () {
    var prebtn_listener = null;
    var nextbtn_listener = null;
    var preIcon = null;
    var nexIcon = null;
    LoadHeaderBtnListener = function (containerId, tabContainerId) {
        const conatiner = document.getElementById(containerId);
        const tabContainer = document.getElementById(tabContainerId);
        const preBtnEle = conatiner.querySelector(".pre-btn")
        const nextBtnEle = conatiner.querySelector(".nex-btn")
        if (!CheckIsoverflow(tabContainer)) {
            if (preBtnEle) {
                var icon = preBtnEle.querySelector("i");
                if (icon) {
                    preIcon = icon;
                    preBtnEle.removeChild(icon);
                }
            }
            if (nextBtnEle) {
                var icon = nextBtnEle.querySelector("i");
                if (icon) {
                    nexIcon = icon;
                    nextBtnEle.removeChild(nexIcon);
                }
            }
            return;
        }
        tabContainer.scrollLeft = tabContainer.scrollWidth - tabContainer.offsetWidth;
        preBtnEle.appendChild(preIcon);
        nextBtnEle.appendChild(nexIcon);
        prebtn_listener = preBtnEle.addEventListener("click", function () {
            var scrollwidth = tabContainer.offsetWidth / 4
            tabContainer.scrollLeft = Math.max(
                tabContainer.scrollLeft - scrollwidth,
                0
            );
        });
        nextbtn_listener = nextBtnEle.addEventListener("click", function () {
            var scrollwidth = tabContainer.offsetWidth / 4
            const maxScrollLeft = tabContainer.scrollWidth - tabContainer.offsetWidth;
            tabContainer.scrollLeft = Math.min(
                tabContainer.scrollLeft + scrollwidth,
                maxScrollLeft
            );
        });
    }
    CheckIsoverflow = function (tabContainer){
        const lis = Array.from(tabContainer.querySelectorAll('.tab-list-ul li'));
        const totalLiWidth = lis.reduce((total, li) => total + li.offsetWidth, 0);
        if (totalLiWidth <= tabContainer.offsetWidth) {
            return false;
        } else {
            return true;
        }
    }
    RemoveHeaderBtnListener = function () {
        if (prebtn_listener != null)
            document.removeEventListener("click", prebtn_listener)
        if (nextbtn_listener != null)
        document.removeEventListener("click", nextbtn_listener)
    }
    Init= function (tabContainerId) {
        var tabContainer = document.getElementById(tabContainerId);
        var tabs = $(tabContainer).find(".tab-content .tab-pane");
        var headers = [];
        var tabIds = [];
        for (var i = 0; i < tabs.length; i++) {
            let tabId = "tab_" + i.toString();
            $(tabs[i]).attr("id", tabId);
            tabIds.push(tabId);
            $(tabs[i]).attr("aria-labelledby", "tab_header_" + i.toString());
            let header = $(tabs[i]).attr("data-tab-header");
            headers.push(header)
            $(tabs[i]).attr("tabindex", i.toString());
        }

        for (var i = 0; i < headers.length; i++) {
            var tabHeader = $('<li class="nav-item" role="presentation"><button class= "nav-link" id="btntab' + i.toString() +
                '" data-bs-toggle="tab" data-bs-target="#' + tabIds[i] + '" type = "button" role = "tab" aria-controls="' + tabIds[i] + '" aria-selected="false" >' + headers[i] + '</button></li> ')
            $(tabContainer).find(".nav").append(tabHeader)
        }

        var activedTab = $(tabContainer).find(".tab-content .active");
        let activeTabId = '';
        if (activedTab.length > 0) {
            activeTabId = activedTab.attr("id");
        }
        else {
            activeTabId = $(tabContainer).find(".tab-content .tab-pane")[0].id;
        }

        $(tabContainer).find(".nav-tabs .nav-item .nav-link[data-bs-target$='" + activeTabId + "']").addClass("active");//[0].click();
        console.log("Init...");
        //console.log(headers);
        //console.log(tabIds);

}
return {
    LoadHeaderBtnListener: LoadHeaderBtnListener,
    RemoveHeaderBtnListener: RemoveHeaderBtnListener,
};
}) ();
