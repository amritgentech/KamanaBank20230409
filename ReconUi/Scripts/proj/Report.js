/// <reference path="JSON-to-Table.min.1.0.0.js" />
var mindate;
var maxdate;
$('#btnShow').click(function () {
    var $d = $.Deferred();
    var rptType = $('.rptType').val();

    var ToDate = $('#max').val();
    var FromDate = $('#min').val();
    var TerminalId = $('.terminalId').val();

    if (FromDate == '' || ToDate == '') {
        alert("Please select date range to generate report.");
        return;
    }

    var url = ['/ReportExports/ShowReports?reportType=' + rptType + '&ToDate=' + ToDate + '&FromDate=' + FromDate + '&terminalId=' + TerminalId]
    $.ajax({
        url: url,
        type: 'GET',

    }, "json").done(function (data) {
        $d.resolve(data);
        var data1 = JSON.parse(data)
        $('#recon-table').createTable(data1);

        $(document).ready(function () {
            var fileName = rptType + "-" + (new Date()).toLocaleDateString('en-US');
            var table = $('#ccoms-table').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'excel',
                        title: fileName
                    }
                ],
                initComplete: function () {
                    this.api().columns().every(function () {
                        var column = this;
                        var select = $('<select class="select2"><option value="">Show All</option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex(
                                    $(this).val()
                                );

                                column
                                    .search(val ? '^' + val + '$' : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d, j) {
                            select.append('<option value="' + d + '">' + d + '</option>')
                        });
                    });
                }
            });
         
            $('.buttons-excel').addClass('btn btn-info');
            $('.buttons-excel').text('Download Excel');
            $('.excbtn').append($('.buttons-excel'));
          //  $('.select2').select2();

        });
    }).fail(function (data) {
        $d.reject(data);
    });
    return $d.promise();

});