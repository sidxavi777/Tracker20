// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// script.js

// Existing code from your script tags

   


$(document).ready(function () {
    $("#filterButton").click(function () {
        var selectedValue = $("#filterByNameDropdown").val();
        $(this).attr("asp-route-ids", selectedValue);

        // Get the HTML content of the div based on the selected value
        var divHtml = $("#" + selectedValue).prop("outerHTML");
        console.log("Div content:", divHtml);
    });
});

function filter() {
    var dropdown = document.getElementById("filterByNameDropdown");
    var selection = dropdown.options[dropdown.selectedIndex].text;
    console.log('Selected: ' + selection);

    var myButton = document.getElementById("filterButton");
    console.log(myButton);
}

// toggleDiv save button
function toggleInputField(tableColumnsId) {
    var commentInput = document.getElementById('commentInput_' + tableColumnsId);

    if (commentInput.style.display === 'none' || commentInput.style.display === '') {
        commentInput.style.display = 'block';
    } else {
        commentInput.style.display = 'none';
    }
}

function saveComment(id) {
    var ticketNum = id;
    var input = $('#commentDescription_' + id).val();
    var ip = {
        commentAjax: input,
        TicketNumAjax: ticketNum
    }

    $.ajax({
        type: "POST",
        url: "Home/Comment", 
        dataType: "json",
        data: ip,
        success: function (res) {
            // Create a new comment element
            var newRow = '<div class="media border p-3 mb-3 ticket-' + res.tId + '">' +
                '<div class="media-body d-flex flex-column text-wrap overflow-auto">' +
                '<p class="mt-0"><b>' + res.name + '</b> <small class="text-muted">' + res.time + '</small></p>' +
                '<p class="mb-1 ' + res.cId + '-p">' + res.desc + '</p>' +
                '<input type="text" class="form-control mb-1 ' + res.cId + '-input" value="' + res.desc + '" style="display: none;">' +
                '<small class="text-muted" style="color: black">' +
                '<a href="#" style="color: black" onclick="editComment(\'' + res.cId + '\', \'' + res.id + '\')">Edit | </a>' +
                '<a href="#" style="color: black" onclick="saveEditedComment(\'' + res.cId + '\', \'' + res.id + '\')">Save | </a>' +
                '<a href="#" style="color: black" onclick="deleteComment(\'' + res.cId + '\', \'' + res.id + '\')">Delete</a>' +
                '</small>' +
                '</div>' +
                '</div>';



            // Append the new comment to the corresponding ticket comments section
            $('.comments-of-ticket_' + res.id).prepend(newRow);
            $('#commentDescription_' + res.id).val("");
        },
        error: function (req, status, error) {
            console.log(status);
        }
    });
}


