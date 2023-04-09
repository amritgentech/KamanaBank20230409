
!function (t) {
    t.fn.createTable = function (e, a) {
        var r, o = this, n = t.extend({}, t.fn.createTable.defaults, a);
        if (void 0 !== o[0].className) {
            var i = o[0].className.split(" "); r = "." + i.join(".") + " "
        }
        else void 0 !== o[0].id && (r = "#" + o[0].id + " ");
        var l = '<table class="jsontable table" id="ccoms-table">';
        return l += '<thead><th class="jsl"></th>',
            l += t.fn.createTable.parseTableData(e, !0),
            l += "</thead>",
            l += "<tbody>",
            l += t.fn.createTable.parseTableData(e, !1),
            l += "</tbody>",
            l += '<tfoot><th></th>',
            l += t.fn.createTable.parseTableData(e, !0),
            l += "</tfoot>",
            l += "</table>",
        o.html(l),
        function () {
        }()
    }, t.fn.createTable.getHighestColumnCount = function (e) {
        for (var a = 0, r = 0, o = { max: 0, when: 0 }, n = 0; n < e.length; n++)
            a = t.fn.getObjectLength(e[n]), a >= r && (r = a, o.max = a, o.when = n);
        return o
    }, t.fn.createTable.parseTableData = function (e, a) {
        for (var r = "", o = 0; o < e.length; o++)
            a === !1 && (r += '<tr><td class="jsl">' + (o + 1) + "</td>"),
            t.each(e[o], function (n, i) {
                a === !0 ? o === t.fn.createTable.getHighestColumnCount(e).when
                && (r += "<th>" + t.fn.humanize(n) + "</th>") : a === !1
                   && (r += "<td>" + i + "</td>")
            }), a === !1 && (r += "</tr>");
        return r
    },

    t.fn.getObjectLength = function (t) {
        var e = 0;
        for (var a in t)
            t.hasOwnProperty(a) && ++e;
        return e
    },
    t.fn.humanize = function (t) { var e = t.split("_"); for (i = 0; i < e.length; i++) e[i] = e[i].charAt(0).toUpperCase() + e[i].slice(1); return e.join(" ") },
    t.fn.createTable.defaults = {
        borderWidth: "1px", borderStyle: "solid", borderColor: "#DDDDDD", fontFamily: "Verdana, Helvetica, Arial, FreeSans, sans-serif", thBg: "#F3F3F3", thColor: "#0E0E0E", thHeight: "30px", thFontFamily: "Verdana, Helvetica, Arial, FreeSans, sans-serif", thFontSize: "14px", thTextTransform: "capitalize", trBg: "#FFFFFF", trColor: "#0E0E0E", trHeight: "25px", trFontFamily: "Verdana, Helvetica, Arial, FreeSans, sans-serif", trFontSize: "13px", trPaddingLeft: "10px", trPaddingRight: "10px"
    }
}(jQuery);