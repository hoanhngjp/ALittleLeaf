$(document).ready(function () {
    $('#btn-complete-order').on('click', function (event) {
        event.preventDefault();
        var selectedMethod = $('input[name="checkoutMethod"]:checked').val();

        var $form = $('#checkout_complete');

        if (selectedMethod === 'VNPAY') {
            $form.attr('action', '/Payment/CreatePaymentUrl');
        }
        else if (selectedMethod === 'cod' || selectedMethod === 'online') {
            $form.attr('action', '/CheckOut/PlaceCodOrder');
        }
        else {
            alert('Vui lòng chọn phương thức thanh toán.');
            return;
        }
        $form.submit();
    });
});