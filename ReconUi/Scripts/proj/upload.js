$(document).ready(function (event) {
    if ($("#hasSubSource").attr("data-val") === "true") {
        getSubSourceInfo();
    }
    if ($("#hasSubChildSource").attr("data-val") === "true") {
        getSubChildSourceInfo();
    }
    $("#drpSubSource").hide();
    $("#drpSubChildSource").hide();
    $("#successNotify").hide();  // hide alert div..
    $("#errorNotify").hide();  // hide alert div..

    //reload all the sources dropdown..
    initializeandReloadUpload();

    function initializeandReloadUpload() {
        if ($("#SourceIdReload").val()) {
            //reload sub source dropdown..
            $('#drpSource').val($("#SourceIdReload").val());

            $('#drpSource').trigger('change');
            reloadUploadFileList($('#drpSource').val(), $('#drpSubSource').val() || "", $('#drpSubChildSource').val() || "");
        }
    }

    $(document).on("change",
        "#drpSource",
        function (event) {
            getSubSourceInfo();
            $("#SubSourceIdValNext").val(''); //reset when source change..
            $("#SubChildSourceIdValNext").val(''); //reset when source change..

            $("#SubSourceIdReload").val(''); //reset when source change..
            $("#SubChildSourceIdReload").val(''); //reset when source change..

            $('#drpSubSource').val("");
            $('#drpSubChildSource').val("");


            $("#drpSubChildSource").hide();

            $("#ViewTransactionsPartial").html("");

            reloadUploadFileList($('#drpSource').val(), $('#drpSubSource').val() || "", $('#drpSubChildSource').val() || "");
        });

    function getSubSourceInfo() {
        $("#SourceName").val($('#drpSource :selected').text());
        $.ajax({
            type: "GET",
            url: "/fileupload/GetSubSource",
            data: { sourceId: $('#drpSource').val() },
            success: function (data) {
                if (data.length > 0) {

                    $("#drpSubSource").html("");
                    var markup = "<option value>Select Sub Source</option>";
                    for (var x = 0; x < data.length; x++) {
                        markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";
                    }
                    $("#drpSubSource").html(markup).show();
                    $("#drpSubSource").show();
                    $('#drpSubSource').get(0).selectedIndex = 0;

                    if ($("#SubSourceIdValNext").val()) // if not then reloaded value
                        $("#drpSubSource").val($("#SubSourceIdValNext").val());
                    else
                        $("#drpSubSource").val($("#SubSourceIdReload").val());

                    $("#drpSubSource").trigger("change");

                    //dropdown subsource fill when reload..
                    if ($("#SubSourceIdReload").val()) {
                        $('#drpSubSource').val($("#SubSourceIdReload").val());
                    }
                } else {
                    $("#drpSubSource").hide();
                }
            }
        });
    }

    function reloadUploadFileList(sourceId, subSourceId, subChildSourceId) {
        $.ajax({
            type: "GET",
            url: "/fileupload/GetUploadHistoryBySourceId",
            data: { sourceId: sourceId, subSourceId: subSourceId, subChildSourceId: subChildSourceId },
            success: function (data) {
                $("#FileUploadedHistoryPartial").html("");
                $("#FileUploadedHistoryPartial").html(data);
            }
        });
    }
    $(document).on("change",
        "#drpSubSource",
        function (event) {

            if (event.originalEvent !== undefined) {  // to find out either original event or trigger event..
                $("#SubChildSourceIdReload").val("");
                $('#drpSubChildSource').val("");
            }
            getSubChildSourceInfo();
            //            $("#SubChildSourceIdValNext").val(''); //reset when source change..
            $("#drpSubChildSource").hide();   //hide drpSubChildSource..

            $("#SubSourceName").val($('#drpSubSource :selected').text());
            $("#SubSourceIdVal").val($('#drpSubSource :selected').val());

            $("#ViewTransactionsPartial").html("");

            reloadUploadFileList($('#drpSource').val(), $('#drpSubSource').val() || "", $('#drpSubChildSource').val() || "");
        });


    function getSubChildSourceInfo() {
        var subSourceId;
        $("#SubSourceName").val($('#drpSubSource :selected').text());
        if ($("#SubChildSourceIdReload").val()) {
            $('#drpSubChildSource').val($("#SubSourceIdReload").val());
            subSourceId = $("#SubSourceIdReload").val();
        } else {
            subSourceId = $('#drpSubSource').val();
        }
        $.ajax({
            type: "GET",
            url: "/fileupload/GetSubChildSource",
            data: { subSourceId: subSourceId },
            success: function (data) {
                if (data.length > 0) {

                    $("#drpSubChildSource").html("");
                    var markup = "<option value>Select Sub Child Source</option>";
                    for (var x = 0; x < data.length; x++) {
                        markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";
                    }
                    $("#drpSubChildSource").html(markup).show();
                    $("#drpSubChildSource").show();
                    $('#drpSubChildSource').get(0).selectedIndex = 0;


                    if ($("#SubChildSourceIdValNext").val()) // if not then reloaded value
                        $("#drpSubChildSource").val($("#SubChildSourceIdValNext").val());
                    else
                        $("#drpSubChildSource").val($("#SubChildSourceIdReload").val());

                    $("#drpSubChildSource").trigger("change");

                    //relaod sub child source dropdown when reload..
                    if ($("#SubChildSourceIdReload").val()) {
                        $('#drpSubChildSource').val($("#SubChildSourceIdReload").val());
                    }
                } else {
                    $("#drpSubChildSource").hide();
                }
            }
        });
    }
    $(document).on("change",
        "#drpSubChildSource",
        function (event) {
            $("#SubChildSourceName").val($('#drpSubChildSource :selected').text());
            $("#SubChildSourceIdVal").val($('#drpSubChildSource :selected').val());

            reloadUploadFileList($('#drpSource').val(), $('#drpSubSource').val() || "", $('#drpSubChildSource').val() || "");
        });
    if ($("#CheckReconActivate").val() === "True") {
        //        $("#btnSubmit").attr('disabled', 'disabled');
        //        $("#info").notify("Please wait....Recon is ongoing.", "info");
    } else if ($("#CheckReconActivate").val() === "False") {
        //        $("#btnSubmit").removeAttr('disabled');
        $("#successNotify").addClass("successAlert");
        $("#successNotify").show();
        ResetReconStatusActivateFlag();
    }
    else if ($("#CheckReconActivate").val() === "Error") {
        $("#btnSubmit").removeAttr('disabled');
        $("#errorNotify").addClass("errorAlert");
        $("#errorNotify").show();
        ResetReconStatusActivateFlag();
    }

    function ResetReconStatusActivateFlag() {
        $.post("/FileUpload/ResetReconActivateFlag", function () {
        });
    }

    //delete file..
    $(document).on("click",
        "#DeleteFileupload",
        function (event) {

            var filename = $(this).closest('tr').find('td').first().html();
            if (!confirm('Are you sure to delete a  file  "' + filename + '" ?')) return;
            $.ajax({
                type: "GET",
                url: "/fileupload/DeleteFileHistory",
                data: { fileId: $(this).attr("fileid") },
                success: function (data) {
                    reloadUploadFileList($('#drpSource').val(), $('#drpSubSource').val() || "", $('#drpSubChildSource').val() || "");
                }
            });
        });

    //View file..
    $(document).on("click",
        "#ViewFileupload",
        function (event) {
            $("#FileId").val($(this).attr("fileid"));
            $.ajax({
                type: "GET",
                url: "/fileupload/ViewTransactions",
                data: { fileId: $(this).attr("fileid") },
                success: function (data) {
                    $("#ViewTransactionsPartial").html("");
                    $("#ViewTransactionsPartial").html(data);
                }
            });
        });

    $(document).on('click', '#btnExport', function () {
        window.location.href = "/Report/ExportFileHistoryRecord?fileId=" + $("#FileId").val();
    });
});