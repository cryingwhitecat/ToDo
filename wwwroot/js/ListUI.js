$("#showNewForm").click(function (e) {
    e.preventDefault();
    $("#modalDialog").modal('show');
});
$("i[name='editItem']").click(function (event) {

    var $target = $(event.target);
    var title = $target.data("title");
    var itemId = $target.data("itemid");

    $("#EditTitleTextBox").val(title);
    $("#EditItemId").val(itemId);
    $("#editItemModalDialog").modal('show');
});
$("i[name='deleteItem']").click(function (event) {
    var $target = $(event.target);
    var id = $target.data("id");
    $("#deleteItemModalDialog").modal('show');
    $("#deleteToDoItem").click(function () {
        var data = { itemId: id };
        $.ajax({
            url: '/Account/Delete',
            data: data,
            contentType: 'application/x-www-form-urlencoded',
            type: 'POST',
            success: function (response) {
                location.reload();
            }
        });
    });
});