function SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, ajaxReturnData) // return scrollPosition
{
    var type = elementId
    var text;
    var sortType;
    var position;
    for (var i = 0; i < (type.length / 2) ; i++) {
        text = type[i].className;

        if (text == 'sorting_asc' || text == 'sorting_desc') {
            position = i;
            sortType = text.replace("sorting_", "");
            break;
        }
    }
    scrollPositionBTBefore = scrollPositionObject
    elementToReplace.replaceWith($(ajaxReturnData));

    if (position != null && sortType != null) {
        window.sortTable.fnSort([[position, sortType]]);
        window.sortTable.fnDraw();
    }

    return scrollPositionBTBefore;
}