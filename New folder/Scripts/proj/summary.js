$(document).ready(function () {
    var form = $('#headerSearch');
    $("#btnExport").hide();
    $("#btnExportMb").hide();
    form.hide();
    var reconTypeMethod = "";
    var dateToday = new Date();
    function TwoWayReconDataTableLoadEsewaVsCbs() {
       
        var oTable = $('#summary-table').dataTable({
            "fnDrawCallback": function (userTable) {
                if (userTable.aoData.length > 0) {
                    $("#btnExportMb").show();
                }
            },
            "paging": true,
            "lengthChange": true,
            "searching": false,
            "ordering": false,
            "info": true,
            "autoWidth": false,
            "serverSide": true,
            "bServerSide": true,
            "sAjaxSource": "/Report/AjaxHandler?" + $('#headerSearch').serialize(),
            "bProcessing": true,
           
            "aoColumnDefs": [
                {
                    "mData": "TransactionDate",
                    "aTargets": [0],
                    "bSortable": false,
                    "bSearchable": false
                },
                {
                    "mData": "UniqueId",
                    "aTargets": [1],
                    "bSortable": false,
                    "bSearchable": false
                }, {
                    "mData": "Amount",
                    "aTargets": [2],
                    "bSortable": false,
                    "bSearchable": false
                }, {
                    "mData": "EsewaSettlement",
                    "aTargets": [3], "bSortable": false,
                    "bSearchable": false
                },
                {
                    "mData": "Status",
                    "aTargets": [4],
                    "bSortable": false,
                    "bSearchable": false,
                }
            ]
        });
        return oTable;
    }
    function TwoWayReconDataTableLoad() {
        var oTable = $('#summary-table').dataTable({
            "fnDrawCallback": function (userTable) {
                if (userTable.aoData.length > 0) {
                    $("#btnExport").show();
                }
            },
            "paging": true,
            "lengthChange": true,
            "searching": false,
            "ordering": false,
            "info": true,
            "autoWidth": false,
            "serverSide": true,
            "bServerSide": true,
            "sAjaxSource": "/Report/AjaxHandler?" + $('#headerSearch').serialize(),
            "bProcessing": true,
            "aoColumnDefs": [
                {
                    "mData": "TransactionDate",
                    "aTargets": [0],
                    "bSortable": false,
                    "bSearchable": false,
                    "type": "date-dd-mmm-yyyy "
                },
                //                {
                //                    "mData": "TransactionDate",
                //                    "aTargets": [0],
                //                    "bSortable": false,
                //                    "bSearchable": false,
                //                    "type": "date-dd-mmm-yyyy "
                //                    ,
                //                    "render": function (data, type, full, meta) {
                //                        return dtConvFromJSON(data);
                //                    }
                //                },
                {
                    "mData": "TerminalId",
                    "aTargets": [1],
                    "bSortable": false,
                    "bSearchable": false
                }, {
                    "mData": "CardNo",
                    "aTargets": [2],
                    "bSortable": false,
                    "bSearchable": false
                }, {
                    "mData": "TraceNo",
                    "aTargets": [3],
                    "bSortable": false,
                    "bSearchable": false
                },
                {
                    "mData": "TransactionAmountSource",
                    "aTargets": [4],
                    "bSortable": false,
                    "bSearchable": false,
                    "className": "leftpadding"
                },
                {
                    "mData": "ResponseCodeSource",
                    "aTargets": [5],
                    "bSortable": false,
                    "bSearchable": false,
                    "className": "center"
                }, {
                    "mData": "StatusSource",
                    "aTargets": [6],
                    "bSortable": false,
                    "bSearchable": false
                },
                {
                    "mData": "TransactionAmountDestination",
                    "aTargets": [7],
                    "bSortable": false,
                    "bSearchable": false,
                    "render": function (data, type, full, meta) {
                        if (data == 0)
                            return "";
                        return data;
                    }
                }, {
                    "mData": "ResponseCodeDestination",
                    "aTargets": [8],
                    "bSortable": false,
                    "bSearchable": false,
                    "className": "center"
                }
            ]
        });
        return oTable;
    }
    function ThreeWayReconDataTableLoad() {
        var oTable = $('#summary-table').dataTable({
            "fnDrawCallback": function (userTable) {
                if (userTable.aoData.length > 0) {
                    $("#btnExport").show();
                }
            },
            "paging": true,
            "lengthChange": true,
            "searching": false,
            "ordering": false,
            "info": true,
            "autoWidth": false,
            "serverSide": true,
            "bServerSide": true,
            "sAjaxSource": "/Report/AjaxHandler?" + $('#headerSearch').serialize(),
            "bProcessing": true,
            "aoColumnDefs": [
                {
                    "mData": "TransactionDate",
                    "aTargets": [0],
                    "bSortable": false,
                    "bSearchable": false
                }, {
                    "mData": "TerminalId",
                    "aTargets": [1],
                    "bSortable": false,
                    "bSearchable": false
                }, {
                    "mData": "CardNo",
                    "aTargets": [2],
                    "bSortable": false,
                    "bSearchable": false
                }, {
                    "mData": "TraceNo",
                    "aTargets": [3],
                    "bSortable": false,
                    "bSearchable": false
                }
                , {
                    "mData": "StatusSource",
                    "aTargets": [4],
                    "bSortable": false,
                    "bSearchable": false
                }, {
                    "mData": "TransactionAmountSource",
                    "aTargets": [5],
                    "bSortable": false,
                    "bSearchable": false,
                    "className": "leftpadding"
                }
                , {
                    "mData": "ResponseCodeSource",
                    "aTargets": [6],
                    "bSortable": false,
                    "bSearchable": false,
                    "className": "center"
                },
                {
                    "mData": "TransactionAmountDestination1",
                    "aTargets": [7],
                    "bSortable": false,
                    "bSearchable": false,
                    "render": function (data, type, full, meta) {
                        if (data == 0)
                            return "";
                        return data;
                    }
                },
                {
                    "mData": "ResponseCodeDestination1",
                    "aTargets": [8],
                    "bSortable": false,
                    "bSearchable": false,
                    "className": "center"
                }
                , {
                    "mData": "TransactionAmountDestination2",
                    "aTargets": [9],
                    "bSortable": false,
                    "bSearchable": false,
                    "render": function (data, type, full, meta) {
                        if (data == 0)
                            return "";
                        return data;
                    }
                }, {
                    "mData": "ResponseCodeDestination2",
                    "aTargets": [10],
                    "bSortable": false,
                    "bSearchable": false,
                    "className": "center"
                }
            ]
        });
        return oTable;
    }
    function dtConvFromJSON(data) {
        if (data == null) return '1/1/1950';
        var r = /\/Date\(([0-9]+)\)\//gi
        var matches = data.match(r);
        if (matches == null) return '1/1/1950';
        var result = matches.toString().substring(6, 19);
        var epochMilliseconds = result.replace(
            /^\/Date\(([0-9]+)([+-][0-9]{4})?\)\/$/,
            '$1');
        var b = new Date(parseInt(epochMilliseconds));
        var c = new Date(b.toString());
        var curr_date = c.getDate();
        var curr_month = c.getMonth() + 1;
        var curr_year = c.getFullYear();
        var curr_h = c.getHours();
        var curr_m = c.getMinutes();
        var curr_s = c.getSeconds();
        var curr_offset = c.getTimezoneOffset() / 60
        //        var d = curr_month.toString() + '/' + curr_date + '/' + curr_year + " " + curr_h + ':' + curr_m + ':' + curr_s;
        var d = curr_month.toString() + '/' + curr_date + '/' + curr_year;
        var dateObj = new Date(d);
        locale = "en-us",
            monthName = dateObj.toLocaleString(locale, { month: "long" });
        return curr_date + ' ' + monthName + ' ' + curr_year;
    }

    $("#drpReconType").change(function () {
        $("#ReconTypeName").val($("#drpReconType option:selected").text());
    });

    $("#btnExport").click(function (e) {
        window.location.href = "/Report/ExportToExcel?" + $('#headerSearch').serialize();
    });
    $("#btnExportMb").click(function (e) {
        window.location.href = "/Report/MobilebankingExportToExcel?" + $('#headerSearch').serialize();
    });
    jQuery(window).load(function () {
        var rtm = $("#ReconTypeName").val();
        var sourceNdestinationName = $("#drpReconType option:selected").text().split('Vs');
        if (sourceNdestinationName.length == 2) {
            if (rtm == "Esewa Vs Cbs")
                TwoWayReconDataTableLoadEsewaVsCbs();
           else if (rtm == "Topup Vs Cbs")
                TwoWayReconDataTableLoadTopupVsCbs();
            else {
                TwoWayReconDataTableLoad();
            }

        }
        else if (sourceNdestinationName.length == 3) {
            ThreeWayReconDataTableLoad();
        }
    });

    $('#FromDate').datepicker({
        inline: true,
        changeYear: true,
        changeMonth: true,
        dateFormat: "dd/mm/yy",
        maxDate: "0",
        onClose: function (selectedDate) {
            $("#ToDate").val($('#FromDate').val());
            $('#ToDate').datepicker('option', 'minDate', selectedDate);
        }
    });
    $('#ToDate').datepicker({
        inline: true,
        changeYear: true,
        changeMonth: true,
        dateFormat: "dd/mm/yy",
        maxDate: '0',
        minDate: dateToday,
        onSelect: function (dateText, inst) {
            //set previous date..
            //            $("#FromDate").datepicker("setDate", -1);
        },
        onClose: function (selectedDate) {
            //            $('#FromDate').datepicker('option', 'minDate', selectedDate);
        }
    });

    $("#FromDate").focus(function () {
        $(this).datepicker();
    });
    $("#ToDate").focus(function () {
        $(this).datepicker();
    });
    $(".txnType").each(function () {
        if (this.id != $("#ReconTypeStatus").val()) {
            $(this).hide();
        }
    });
    $(".totalTransaction").hide();
    var uri = window.location.toString();
    if (uri.indexOf("?") > 0) {
        var clean_uri = uri.substring(0, uri.indexOf("?"));
        window.history.replaceState({}, document.title, clean_uri);
    }
});
