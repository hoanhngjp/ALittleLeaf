document.addEventListener('DOMContentLoaded', function() {
    var menuItems = document.querySelectorAll('.menu-item');

    menuItems.forEach(function(item) {
        item.addEventListener('click', function(event) {
            var subMenu = item.querySelector('.sub-menu');
            if (subMenu) {
                if (subMenu.classList.contains('active')) {
                    subMenu.classList.remove('active'); // Ẩn sub-menu nếu đã hiển thị
                } else {
                    // Ẩn tất cả các sub-menu khác trước khi hiển thị sub-menu mới
                    document.querySelectorAll('.sub-menu').forEach(function(menu) {
                        menu.classList.remove('active');
                    });
                    subMenu.classList.add('active'); // Hiển thị sub-menu
                }
            }
        });
    });

    // Bổ sung xử lý sự kiện cho các liên kết trong .sub-menu
    var subMenuLinks = document.querySelectorAll('.sub-menu a');
    subMenuLinks.forEach(function(link) {
        link.addEventListener('click', function(event) {
            // Cho phép hành động mặc định của liên kết xảy ra (chuyển hướng trang)
            // Bỏ qua việc ngăn chặn mặc định của sự kiện click
        });
    });
});
