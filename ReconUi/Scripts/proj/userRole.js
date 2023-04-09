$(document).ready(function (event) {
    var baseUrl = document.location.origin;
    $(".btnSave").hide();
    $(".EditUserRole").click(function () {
        //        $(this).hide();
        $(".btnSave").hide();
        $(".EditUserRole").show();


        var edit_role_id = $(this).attr("edit-role-Id");
        $(".btnSave").each(function () {
            if ($(this).attr("but-roleId") === edit_role_id) {
                $(this).parent().find('[edit-role-Id="' + edit_role_id + '"]').hide(); // hide edit button..
            }
        });
        $('[but-roleId="' + edit_role_id + '"]').show();
        //        $(this).hide();


        $(".clsUsername").each(function () {
            $(this).attr("disabled", "disabled");
        });
        $(".clsDrpRoles").each(function () {
            $(this).attr("disabled", "disabled");
        });
        $(this).closest('tr').find("#UserName").removeAttr("disabled");
        $(this).closest('tr').find("#UserRoleId").removeAttr("disabled");
    });

    $(".btnSave").click(function (e) {
        $(".btnSave").hide();
        $("#btn+" + this.id).show();


        userid = $(this).attr("but-userId");
        username = $(this).closest('tr').find("#UserName").val();
        roleId = $(this).closest('tr').find("#UserRoleId").val();
        $.ajax({
            url: baseUrl + '/user/UpdateUserRole?UserId=' + userid + '&UserName=' + username + '&UserRoleId=' + roleId,
            dataType: 'text',
            type: 'post',
            contentType: 'application/x-www-form-urlencoded',
            success: function (data, textStatus, jQxhr) {
                $(".EditUserRole").show();
                $("#userRoleTable").html("");
                $("#userRoleTable").html(data);
                window.location.href = baseUrl + "/user/index";
            },
            error: function (jqXhr, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
        e.preventDefault();
    });
});