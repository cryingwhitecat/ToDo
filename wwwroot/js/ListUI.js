//shows modal window which contains form for a new item
$("#showNewForm").click(function (e) {
    e.preventDefault();
    $("#modalDialog").modal('show');
});
//shows modal window which contains form used for editing existing items
$("i[name='editItem']").click(function (event) {

    var $target = $(event.target);    //get specific <a>
    var title = $target.data("title"); //get item title and id from the data-* attributes
    var itemId = $target.data("itemid"); 

    $("#EditTitleTextBox").val(title);  // populate input fields with the pre-existing item's data
    $("#EditItemId").val(itemId);
    $("#editItemModalDialog").modal('show'); //show modal window
});
//shows delete action confirmation form
$("i[name='deleteItem']").click(function (event) {
    var $target = $(event.target); //obtaining a target item's id
    var id = $target.data("id");
    $("#deleteItemModalDialog").modal('show');
    $("#deleteToDoItem").click(function () { // if user confirms the action 
        var data = { itemId: id }; // prepare and execute a POST request
        $.ajax({
            url: '/Account/Delete',
            data: data,
            contentType: 'application/x-www-form-urlencoded', //can't use application/json for primitive types
            type: 'POST',
            success: function (response) { // if all went well, reload the list
                location.reload();
            }
        });
    });
});