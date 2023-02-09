function getSelector(node) {
    var id = node.getAttribute('id');

    if (id) {
        return '#' + id;
    }

    var path = '';

    while (node) {
        var name = node.localName;
        var parent = node.parentNode;

        if (!parent) {
            path = name + ' > ' + path;
            continue;
        }

        if (node.getAttribute('id')) {
            path = '#' + node.getAttribute('id') + ' > ' + path;
            break;
        }

        var sameTagSiblings = [];
        var children = parent.childNodes;
        children = Array.prototype.slice.call(children);

        children.forEach(function (child) {
            if (child.localName == name) {
                sameTagSiblings.push(child);
            }
        });

        // if there are more than one children of that type use nth-of-type

        if (sameTagSiblings.length > 1) {
            var index = sameTagSiblings.indexOf(node);
            name += ':nth-of-type(' + (index + 1) + ')';
        }
        if (path) {
            path = name + ' > ' + path;
        } else {
            path = name;
        }

        node = parent;
    }

    return path;
}
function getTextAreaByValue(value) {
    var textareas = document.getElementsByTagName("d-textarea");
    var result = null;

    for (var x = 0; x < textareas.length; x++) {
        if (textareas[x].value == value) {
            result = textareas[x];

            break;
        }
    }
    return result;
}
var textarea = getTextAreaByValue("banana")
var selector;

if (textarea == null) {
    selector = "";
}
else {
    selector = getSelector(textarea);
}
selector