
var VisibleColumns = [];
var SaveColumnsName = true;
function ApplyResizeDesign(GridID) {
    var grid = $("#" + GridID).data("kendoGrid");
    var screenWidth = parseInt($(window).width());
    var maximumNumberOfColumns = parseInt(screenWidth / 90) - 2;

    //Get Visible Columns name in order to know wish column to show when resize window after hiding all columns only on first load
    if (SaveColumnsName) {
        for (var i = 0 ; i < grid.columns.length ; i++) {
            try {
                if (!grid.columns[i].hidden) {
                    VisibleColumns.push(grid.columns[i].field);
                    SaveColumnsName = false;
                }
            }
            catch (e) { }
        }
    }

    if (maximumNumberOfColumns > 1) {
        for (var i = 0 ; i < grid.columns.length ; i++) {
            try {
                if (VisibleColumns.indexOf(grid.columns[i].field.toString()) > -1)
                    grid.showColumn(i);
            }
            catch (e) { }
        }
    }
    if (grid.columns.length > maximumNumberOfColumns) {
        if (maximumNumberOfColumns > 1) {
            for (var i = maximumNumberOfColumns ; i < grid.columns.length ; i++) {
                try {
                    grid.hideColumn(i);
                }
                catch (e) { }
            }
        }
    }
}

function xs_ExpandTemplate() {
    var screenWidth = parseInt($(window).width());
    if (screenWidth < 700) {
        this.expandRow(this.tbody.find("tr.k-master-row"));
    }
}