(function ($) {
    $(function () {
        const container = $("#sweets-list-container");

        if (!container.length) {
            return;
        }

        const showError = function (message) {
            const alert = `<div class="alert alert-danger" role="alert">
                               <i class="bi bi-exclamation-triangle-fill me-2"></i>${message}
                           </div>`;
            container.html(alert);
        };

        container.on("click", ".pagination .page-link", function (event) {
            const href = $(this).attr("href");

            if (!href || href === "#") {
                event.preventDefault();
                return;
            }

            event.preventDefault();

            container.addClass("opacity-50");

            container.load(href, function (response, status, xhr) {
                container.removeClass("opacity-50");

                if (status === "error") {
                    const message = xhr && xhr.status === 0
                        ? "Не удалось загрузить данные. Проверьте подключение."
                        : `Ошибка загрузки страницы (код: ${xhr.status}).`;
                    showError(message);
                }
            });
        });
    });
})(jQuery);

