﻿var MPDropdownlist = (function () {

    var isVisible = [];
    var clickLFunc = [];
    var keydownFunc = [];
    var hideFunc = [];
    var options = [];
    function show(triggerId, optionPopupEle, filterInputId, searchable) {
        const triggerEle = document.getElementById(triggerId);
        const triggerRect = triggerEle.getBoundingClientRect();
        const filterEle = document.getElementById(filterInputId);

        optionPopupEle.style.display = "block";
        optionPopupEle.style.visibility = 'hidden';
        optionPopupEle.style.width = `${triggerRect.width}px`;
        optionPopupEle.style.left = `${triggerRect.x}px`;

        const viewportHeight = window.innerHeight;
        const optionPopupEleRect = optionPopupEle.getBoundingClientRect();
        let popupHeight = 0;
        if (optionPopupEleRect.height > 5) {//for custom options
            popupHeight = optionPopupEleRect.height
        } else {//for datasource, the options load slowly
            const optionContainerEle = optionPopupEle.querySelector('.dropdown-options-container');
            const optionContainerEleRect = optionContainerEle.getBoundingClientRect();
            popupHeight = optionContainerEleRect.height + (searchable ? 54 : 0);//54 means search box occupying height
        }
        optionPopupEle.style.height = `${popupHeight}px`;

        if (triggerRect.y + triggerRect.height + popupHeight > viewportHeight) {
            //when list-popup overview(downward), make it upward
            optionPopupEle.style.bottom = `${viewportHeight - triggerRect.y + 15}px`;
        } else {
            optionPopupEle.style.top = `${triggerRect.y + triggerRect.height}px`;
        }
        if (!isElementInView(optionPopupEle)) {//means the upper and lower areas are not enough to accommodate the popup
            optionPopupEle.style.top = `${triggerRect.top - (popupHeight - triggerRect.height) / 2}px`
        }

        optionPopupEle.style.visibility = '';

        const hideFuntion = async function (e) {
            if (isVisible[triggerId]) {
                await window.getDotNetRef(triggerId).invokeMethodAsync('HideOptionList');
                optionPopupEle.style.visibility = 'hidden';
                isVisible[triggerId] = false;
                if (clickLFunc[triggerId]) {
                    document.removeEventListener('click', clickLFunc[triggerId])
                    document.removeEventListener('focusout', clickLFunc[triggerId])
                    window.removeEventListener('wheel', clickLFunc[triggerId]);
                }
                if (keydownFunc[triggerId]) {
                    document.removeEventListener('keydown', keydownFunc[triggerId])
                }
                if (hideFunc[triggerId]) {
                    window.removeEventListener('resize', hideFunc[triggerId]);
                }
                options.forEach(x => x.classList.remove('dropdown-option-hover'));
            }
        }

        const clickFunction = async function (event) {
            event.stopPropagation();
            if (event.target == triggerEle || triggerEle.contains(event.target) || event.target == optionPopupEle || optionPopupEle.contains(event.target)) {
                return;
            } else {
                await hideFuntion(event);
            }
        }


        const keydownFunction = async function (event) {
            if (event.key == "Tab") {
                options = optionPopupEle.querySelectorAll(".dropdown-option-container");
                await hideFuntion(event);
                return;
            }
            else if (document.activeElement == filterEle) {//cause search surpport 'enter'
                options = optionPopupEle.querySelectorAll(".dropdown-option-container");
                if (event.key == "ArrowDown") {
                    event.preventDefault();
                    if (options.length > 0) {
                        options[0].classList.add('dropdown-option-hover');
                        filterEle.blur();
                    }
                } else if (event.key == "ArrowUp") {
                    event.preventDefault();
                    return
                }
            } else if (event.key == "ArrowDown" || event.key == "ArrowUp" || event.code == "Space" || event.code == "Enter") {
                event.preventDefault();
                options = optionPopupEle.querySelectorAll(".dropdown-option-container");
                const currentHover = optionPopupEle.querySelector('.dropdown-option-hover');
                var originateIndex = -1;
                var newIndex = 0;
                var unselectedFirstIndex = -1;

                const currentSelected = optionPopupEle.querySelectorAll('.dropdown-option-selected');
                if (currentSelected.length > 1) {//means multi
                    const unselected = Array.from(options).filter(option => {
                        return !Array.from(currentSelected).some(selected => selected === option);
                    });
                    if (unselected.length > 0)
                        unselectedFirstIndex = [...options].indexOf(unselected[0]);
                }

                if (currentHover) {
                    originateIndex = [...options].indexOf(currentHover);
                } else if (unselectedFirstIndex > -1) {
                    originateIndex = unselectedFirstIndex;
                } else {
                    originateIndex = -1;
                }

                if (event.key == "ArrowDown") {
                    if (!currentHover && unselectedFirstIndex > -1 && originateIndex == unselectedFirstIndex) {
                        newIndex = originateIndex;
                    }
                    else if (originateIndex == -1) {
                        newIndex = 0;
                    }
                    else if (originateIndex == options.length - 1) {
                        newIndex = 0;
                    } else {
                        newIndex = originateIndex + 1;
                    }
                }
                else if (event.key == "ArrowUp") {
                    if (originateIndex == -1) {
                        newIndex = 0;
                    }
                    else if (originateIndex == 0) {
                        newIndex = options.length - 1
                    } else {
                        newIndex = originateIndex - 1;
                    }
                }
                else if (event.code == "Space") {
                    if (originateIndex >= 0) {
                        await window.getDotNetRef(triggerId).invokeMethodAsync('UpdateSelectedValueIndex', originateIndex);
                        options[originateIndex].classList.remove('dropdown-option-hover');
                        return;
                    }
                }
                else if (event.code == "Enter") {
                    if (document.activeElement != filterEle && originateIndex >= 0) {
                        await window.getDotNetRef(triggerId).invokeMethodAsync('UpdateSelectedValueIndex', originateIndex);
                        options[originateIndex].classList.remove('dropdown-option-hover');
                    }
                    await hideFuntion(event);
                    return;
                }
                if (originateIndex >= 0) options[originateIndex].classList.remove('dropdown-option-hover');
                options[newIndex].classList.add('dropdown-option-hover');
                options[newIndex].scrollIntoView({
                    block: "center",
                    behavior: "smooth",
                    relativeTo: optionPopupEle
                });
            }
        }

        document.addEventListener('keydown', keydownFunction)
        document.addEventListener('click', clickFunction)
        document.addEventListener('focusout', clickFunction)
        window.addEventListener('resize', hideFuntion);
        window.addEventListener('wheel', clickFunction);


        keydownFunc[triggerId] = keydownFunction;
        clickLFunc[triggerId] = clickFunction;
        hideFunc[triggerId] = hideFuntion;
        isVisible[triggerId] = true;
    }

    function isElementInView(element) {
        var rect = element.getBoundingClientRect();

        var inView = rect.top >= 0;
        inView = inView && rect.bottom <= (window.innerHeight || document.documentElement.clientHeight);

        inView = inView && rect.top <= (window.innerHeight || document.documentElement.clientHeight) + window.scrollY;
        return inView;
    }

    function hide(triggerId, optionPopupEle) {
        if (isVisible[triggerId]) {
            //optionPopupEle.style.display = "none";
            optionPopupEle.style.visibility = 'hidden';
            isVisible[triggerId] = false;
            if (clickLFunc[triggerId]) {
                document.removeEventListener('click', clickLFunc[triggerId]);
                document.removeEventListener('focusout', clickLFunc[triggerId])
                window.removeEventListener('wheel', clickLFunc[triggerId]);
            }
            if (keydownFunc[triggerId]) {
                document.removeEventListener('keydown', keydownFunc[triggerId])
            }
            if (hideFunc[triggerId]) {
                window.removeEventListener('resize', hideFunc[triggerId]);
            }
            options.forEach(x => x.classList.remove('dropdown-option-hover'));
        }
    }

    return {
        show: show,
        hide: hide
    };
})();