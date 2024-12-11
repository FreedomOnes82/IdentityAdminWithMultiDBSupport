var MPPOPUP = (function () {
    var backDropEleList = {};
    function showById(popupElement, targetElementId, position, popupElementId, boxShadowSettings, spacing) {
        var targetElement = document.getElementById(targetElementId);
        show(popupElement, targetElement, position, popupElementId, boxShadowSettings, spacing)
        return null
    }
    function show(popupElement, targetElement, position, popupElementId, boxShadowSettings, spacing) {
        const targetRect = targetElement.getBoundingClientRect();
        const divWidth = parseInt(popupElement.style.width.replace('px', ''));
        const divHeight = parseInt(popupElement.style.height.replace('px', ''));
        if (position.startsWith('left') && targetRect.left - divWidth < 0) {
            position = position.replace('left', 'right');
        }
        const viewportWidth = window.innerWidth || document.documentElement.clientWidth;

        if (position.startsWith('right') && targetRect.right + divWidth > viewportWidth) {
            position = position.replace('right', 'left');
        }
        if (position.startsWith('top') && targetRect.top - divHeight < 0)
            position = position.replace('top', 'bottom');
        const viewportHeight = window.innerWidth || document.documentElement.clientWidth;
        if (position.startsWith('bottom') && targetRect.bottom + divHeight > viewportHeight)
            position = position.replace('bottom', 'top');
        let newTop, newLeft;
        switch (position) {
            case 'left-top':
                newTop = targetRect.top;
                newLeft = targetRect.left - divWidth;
                break;
            case 'left-middle':
                newTop = targetRect.top + (targetRect.height - divHeight) / 2;
                newLeft = targetRect.left - divWidth;
                break;
            case 'left-bottom':
                newTop = targetRect.top + (targetRect.height - divHeight);
                newLeft = targetRect.left - divWidth;
                break;
            case 'right-top':
                newTop = targetRect.top;
                newLeft = targetRect.right;
                break;
            case 'right-middle':
                newTop = targetRect.top + (targetRect.height - divHeight) / 2;
                newLeft = targetRect.right;
                break;
            case 'right-bottom':
                newTop = targetRect.top + (targetRect.height - divHeight);
                newLeft = targetRect.right;
                break;

            case 'top-left':
                newTop = targetRect.top - divHeight;
                newLeft = targetRect.left;
                break;
            case 'top-center':
                newTop = targetRect.top - divHeight;
                newLeft = targetRect.left + (targetRect.width - divWidth) / 2;
                break;
            case 'top-right':
                newTop = targetRect.top - divHeight;
                newLeft = targetRect.right - divWidth;
                break;

            case 'bottom-left':
                newTop = targetRect.bottom;
                newLeft = targetRect.left;
                break;
            case 'bottom-center':
                newTop = targetRect.bottom;
                newLeft = targetRect.left + (targetRect.width - divWidth) / 2;
                break;
            case 'bottom-right':
                newTop = targetRect.bottom;
                newLeft = targetRect.right - divWidth;
                break;
            default:
                console.error('Invalid position:', position);
                return;
        }
        
        popupElement.style.top = `${newTop}px`;
        popupElement.style.left = `${newLeft}px`;
        popupElement.style.display = "block";
        popupElement.style.boxShadow = boxShadowSettings
        if (position.startsWith("top")) {
            popupElement.style.marginBottom = `${spacing}px`;
        } else if (position.startsWith("bottom")) {
            popupElement.style.marginTop = `${spacing}px`;
        } else if (position.startsWith("right")) {
            popupElement.style.marginLeft = `${spacing}px`;
        } else if (position.startsWith("left")) {
            popupElement.style.marginRight = `${spacing}px`;
        }

        var newDiv = document.createElement("div");
        newDiv.className = "mp-backdrop";
        document.body.appendChild(newDiv);

        var hideFuntion = async function (e) {
            e.stopPropagation();
            await window.getDotNetRef(popupElementId).invokeMethodAsync('SetPopupInvisible');
            popupElement.style.display = "none";
            backDropEleList[popupElementId] = null;
            if (document.body.contains(newDiv))
                document.body.removeChild(newDiv);
        }
        newDiv.addEventListener('click', hideFuntion);
        newDiv.addEventListener('wheel', hideFuntion, { passive: true });
        popupElement.addEventListener('wheel', hideFuntion, { passive: true });
        newDiv.addEventListener('resize', hideFuntion);
        backDropEleList[popupElementId] = newDiv;

    }
    function hide(popupElementId, popupEle) {
        popupEle.style.display = "none";
        $(popupEle).removeClass("popup-active");
        if (backDropEleList[popupElementId]&&document.body.contains(backDropEleList[popupElementId])) {
            document.body.removeChild(backDropEleList[popupElementId]);
            backDropEleList[popupElementId] = null;
        }
    }

    function initialTriggerEventById(triggerElementId, popupElementId) {
        var triggerElement = document.getElementById(triggerElementId);
        document.addEventListener('click', async function (event) {
            if (event.target == triggerElement || triggerElement.contains(event.target)) {
                try {
                    await window.getDotNetRef(popupElementId).invokeMethodAsync('ClickTrigger');
                } catch (e) {
                    console.error(e);
                }
            }
        })
    }
    return {
        showById: showById,
        hide: hide,
        initialTriggerEventById: initialTriggerEventById
    };
})();

