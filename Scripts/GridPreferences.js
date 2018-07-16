
function SavePreferences(IsCtcGrid) {
    console.log(window.location.href);
    var ReloadCurrentPage = false;
    if (window.location.href.indexOf("Preferences") == -1)
        ReloadCurrentPage = true;

    var notification = $("#notification").data("kendoNotification");
    var DynamicContactGridColumns = [];
    var ColName;
    var IsVisible;
    var OrderID = 1;
    if (IsCtcGrid) {
        $('[id^="CtcDynamicGrid-"]').each(function () {
            id = $(this)[0].id;
            ColName = id.split('-')[1];
            IsHidden = !$('#' + id + ' input[id="CBCtc_' + ColName + '"]').is(':checked');

            DynamicContactGridColumns.push({
                "ColumnName": ColName,
                "Code": ColName.split('_')[1],
                "IsHidden": IsHidden,
                "OrderID": OrderID++
            });
        });
    } else {
        $('[id^="CpyDynamicGrid-"]').each(function () {
            id = $(this)[0].id;
            ColName = id.split('-')[1];
            IsHidden = !$('#' + id + ' input[id="CBCpy_' + ColName + '"]').is(':checked');

            DynamicContactGridColumns.push({
                "ColumnName": ColName,
                "Code": ColName.split('_')[1],
                "IsHidden": IsHidden,
                "OrderID": OrderID++
            });
        });
    }

    $.ajax({
        url: '/SCV/SaveGridPreferences',
        type: 'POST',
        data: {
            'GridColumns': DynamicContactGridColumns,
            'IsCtcGrid': IsCtcGrid
        },
        success: function (data) {
            if (data == "Success") {
                notification.show({
                    message: "Preferences saved"
                }, "notification-success");
                if (ReloadCurrentPage)
                    location.reload();
            } else {
                notification.show({
                    message: "Error"
                }, "error");
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}
