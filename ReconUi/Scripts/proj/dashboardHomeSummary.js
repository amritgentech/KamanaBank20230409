$(document).ready(function () {
    var form = $('#headerSearch');
    var baseUrl = document.location.origin;
    var dateToday = new Date();
    $("#btnSummaryExport").hide();
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
    $("#ReconTypeName").val($("#drpReconType option:selected").text()); //default set value when page reload back..

    $("#drpReconType").change(function (e) {
        $("#ReconTypeName").val($("#drpReconType option:selected").text());
        e.preventDefault();
    });

    //On us field hide in 3 way summary dashboard ..
    debugger
    var sourceNdestinationName = $("#drpReconType option:selected").text().split('Vs');

    if (sourceNdestinationName.length == 2 && sourceNdestinationName[0].trim() == "Visa") {
        // for visa vs cbs  need to hide onus..
        $(".OnUsScope").hide();
    }

    else if (sourceNdestinationName.length == 3 && sourceNdestinationName[2].trim() == "Visa") {
        $(".OnUsScope").hide();
    }

    else if (sourceNdestinationName.length == 3 && sourceNdestinationName[2].trim() == "Ej") {
        $(".Payable").hide();
    }
    //else
    //    $(".OnUsScope").show();


    $(document).on("click", ".txnType", function (e) {
        var amountAccordingToStatus = $(this).attr("data-amount");
        var statusTotalTxnRecord = $(this).attr("data-val");
        if (statusTotalTxnRecord <= 0) { // prevent from redirect if value is 0 
            return;
        }
        $("#ReconTypeStatus").val(this.id);

        $("#IsOwnUsPayableReceivable").val($(this).parent().parent().attr("id"));
        redirect(amountAccordingToStatus, statusTotalTxnRecord);
    });
    //var redirect = function RedirectToReport(amountAccordingToStatus, statusTotalTxnRecord) {
    //    if ($("#FromDate").val() != '' && $("#ToDate").val() != '' && $("#drpReconType").val() != '') {
    //        $('#loading').show(); // load wait..
    //        window.location = baseUrl + "/Report/Index?" + form.serialize() + "&amountAccordingToStatus=" + amountAccordingToStatus + "&statusTotalTxnRecord=" + statusTotalTxnRecord;
    //    }
    //}
    var redirect = function RedirectToReport(amountAccordingToStatus, statusTotalTxnRecord) {
        debugger
        if ($("#FromDate").val() != '' && $("#ToDate").val() != '' && $("#drpReconType").val() != '') {
            // $('#loading').show(); // load wait..
            let link = window.open(baseUrl + "/Report/Index?" + form.serialize() + "&amountAccordingToStatus=" + amountAccordingToStatus + "&statusTotalTxnRecord=" + statusTotalTxnRecord, "_blank");
            link.opener = null;
        }
    }

    $("#btnSummaryExport").click(function (e) {
        window.location.href = "/Report/ExportSummeryReport?" + $('#headerSearch').serialize();
    });
    $(".numbers").each(function () {
        var flag = 0;
        if (sourceNdestinationName.length == 2) {
            flag = 1;
        }
        else if (sourceNdestinationName.length == 3 && sourceNdestinationName[2].trim() != "Ej") {
            flag = 1;
        }

        if ($(this).find('p').first().text() != 0 && flag == 1) {
            $("#btnSummaryExport").show();
        };
    });
});