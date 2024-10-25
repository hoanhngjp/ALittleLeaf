    document.addEventListener('DOMContentLoaded', function() {
        var thumbPlaceholders = document.querySelectorAll('.thumb-placeholder');
        var productImgFeature = document.querySelector('.product-img-feature');

        thumbPlaceholders.forEach(function(thumbPlaceholder) {
            thumbPlaceholder.addEventListener('click', function(event) {
                event.preventDefault();
                var imgSrc = this.querySelector('img').getAttribute('src');
                productImgFeature.setAttribute('src', imgSrc);
            });
        });
    });

