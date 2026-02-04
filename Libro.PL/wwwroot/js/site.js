var table;
var datatable;
var updatedRow;
var exportedCols = [];

// ======== SweetAlert Messages ========
function showSuccessMessage(message = 'Saved successfully!') {
    Swal.fire({
        icon: 'success',
        title: 'Success',
        text: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

function showErrorMessage(xhr) {
    var message = xhr.responseText || 'Something went wrong!';
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

//function showErrorMessage(xhr) {
//    var message = xhr.responseJSON?.message || 'Something went wrong!';
//    Swal.fire({
//        icon: 'error',
//        title: 'Oops...',
//        html: message,
//        customClass: { confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary" }
//    });
//}

// ======== Handle Modal ========
function onModalBegin() {
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}
function onModalSuccess(row) {
    showSuccessMessage();
    $('#Modal').modal('hide');

    if (updatedRow !== undefined) {
        datatable.row(updatedRow).remove().draw();
        updatedRow = undefined;
    }

    var newRow = $(row);
    datatable.row.add(newRow).draw();

    KTMenu.init();
    KTMenu.initHandlers();
}
function onModalComplete() {
    $('body :submit').removeAttr('disabled').removeAttr('data-kt-indicator');
}

// ======== DataTables ========
var headers = $('th');
$.each(headers, function (i) {
    if (!$(this).hasClass('js-no-export'))
        exportedCols.push(i);
});

// Class definition
var KTDatatables = function () {
    // Private functions
    var initDatatable = function () {
        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "info": false,
            'pageLength': 10,
            "order": [[0, "asc"]]   // ترتيب حسب أول عمود (Id)
        });
    }

    // Hook export buttons
    var exportButtons = () => {
        const documentTitle = $('.js-datatables').data('document-title') + '-Libro';
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));

        // Hook dropdown menu click event to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                // Get clicked export value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger click event on hidden datatable export buttons
                target.click();
            });
        });
    }

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-datatables');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
        }
    };
}();

// ======== Document Ready ========
$(document).ready(function () {
    //TinyMCE
    var options = { selector: ".js-tinymce", height: "422" };

    if (KTThemeMode.getMode() === "dark") {
        options["skin"] = "oxide-dark";
        options["content_css"] = "dark";
    }

    tinymce.init(options);

    //Select2
    $('.js-select2').select2();

    //Datepicker
    $('.js-datepicker').daterangepicker({
        singleDatePicker: true,
        autoApply: true,
        drops: 'up',
        maxDate: new Date()
    });


    // SweetAlert::Show success 
    var message = $('#Message').text().trim();
    if (message !== '') {
        showSuccessMessage(message);
    }

    //DataTables
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });

    // Handle Bootstrap modal rendering
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');

        modal.find('#ModalLabel').text(btn.data('title') || 'Title');

        // Determine if updating an existing row
        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        }

        var url = btn.data('url');
        if (!url || url.endsWith('/0')) {
            showErrorMessage('Invalid URL or ID.');
            return;
        }

        // Fetch form via AJAX
        $.get({
            url: url,
            success: function (form) {
                if (form) {
                    modal.find('.modal-body').html(form);
                    $.validator.unobtrusive.parse(modal);
                } else {
                    showErrorMessage('Empty response from server.');
                }
            },
            error: function () {
                showErrorMessage('Failed to load form.');
            }
        });

        // Show the modal
        modal.modal('show');
    });

    // ======== Handle Toggle Status ========
    $('body').delegate('.js-toggle-status', 'click', function () {
        var btn = $(this);

        bootbox.confirm({
            message: "Are you sure that you need to toggle this item status?",
            buttons: {
                confirm: { label: 'Yes', className: 'btn-danger' },
                cancel: { label: 'No', className: 'btn-secondary' }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (UpdatedOn) {
                            var row = btn.parents('tr');
                            var status = row.find('.js-status');
                            var newStatus = status.text().trim() === 'Deleted' ? 'Available' : 'Deleted';
                            status.text(newStatus).toggleClass('badge-light-success badge-light-danger');
                            row.find('.js-updated-on').html(UpdatedOn);
                            row.addClass('animate__animated animate__flash');

                            showSuccessMessage();
                        },
                        error: function () {
                            showErrorMessage('Failed to toggle status.');
                        }
                    });
                }
            }
        });
    });
});
