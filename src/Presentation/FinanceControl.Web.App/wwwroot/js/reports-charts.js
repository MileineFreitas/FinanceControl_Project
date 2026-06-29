(function () {
    var data = window.repCharts;
    if (!data || typeof Chart === 'undefined') return;

    var palette = [
        '#418be0', '#7dd3fc', '#34d399', '#fbbf24', '#f472b6',
        '#a78bfa', '#fb923c', '#2dd4bf', '#94a3b8', '#ef4444'
    ];

    var locale = (data && data.locale) || 'pt-BR';
    var currencyCode = (data && data.currency) || 'BRL';
    var currency = new Intl.NumberFormat(locale, { style: 'currency', currency: currencyCode });

    Chart.defaults.color = 'rgba(148, 163, 184, 0.95)';
    Chart.defaults.borderColor = 'rgba(65, 139, 224, 0.15)';
    Chart.defaults.font.family = '"Inter", system-ui, sans-serif';

    if (data.mode === 'grouped') {
        renderGroupedCharts(data);
    } else if (data.mode === 'transactions') {
        renderTransactionCharts(data);
    }

    function renderGroupedCharts(payload) {
        var doughnutEl = document.getElementById('rep-chart-despesas');
        if (doughnutEl && payload.despesas && payload.despesas.labels && payload.despesas.labels.length) {
            new Chart(doughnutEl, {
                type: 'doughnut',
                data: {
                    labels: payload.despesas.labels,
                    datasets: [{
                        data: payload.despesas.values,
                        backgroundColor: palette,
                        borderWidth: 2,
                        borderColor: 'rgba(15, 23, 42, 0.85)',
                        hoverOffset: 6
                    }]
                },
                options: doughnutOptions()
            });
        }

        var barEl = document.getElementById('rep-chart-comparativo');
        if (barEl && payload.comparativo && payload.comparativo.labels && payload.comparativo.labels.length) {
            new Chart(barEl, {
                type: 'bar',
                data: {
                    labels: payload.comparativo.labels,
                    datasets: [
                        {
                            label: 'Receitas',
                            data: payload.comparativo.receitas,
                            backgroundColor: 'rgba(52, 211, 153, 0.75)',
                            borderColor: 'rgba(52, 211, 153, 1)',
                            borderWidth: 1,
                            borderRadius: 6,
                            barThickness: 18
                        },
                        {
                            label: 'Despesas',
                            data: payload.comparativo.despesas,
                            backgroundColor: 'rgba(248, 113, 113, 0.75)',
                            borderColor: 'rgba(248, 113, 113, 1)',
                            borderWidth: 1,
                            borderRadius: 6,
                            barThickness: 18
                        }
                    ]
                },
                options: horizontalBarOptions()
            });
        }
    }

    function renderTransactionCharts(payload) {
        var tipoEl = document.getElementById('rep-chart-tipo');
        if (tipoEl && payload.tipo && payload.tipo.values) {
            var hasValue = payload.tipo.values.some(function (v) { return v > 0; });
            if (hasValue) {
                new Chart(tipoEl, {
                    type: 'doughnut',
                    data: {
                        labels: payload.tipo.labels,
                        datasets: [{
                            data: payload.tipo.values,
                            backgroundColor: ['rgba(52, 211, 153, 0.85)', 'rgba(248, 113, 113, 0.85)'],
                            borderWidth: 2,
                            borderColor: 'rgba(15, 23, 42, 0.85)',
                            hoverOffset: 6
                        }]
                    },
                    options: doughnutOptions()
                });
            }
        }

        var dailyEl = document.getElementById('rep-chart-diario');
        if (dailyEl && payload.diario && payload.diario.labels) {
            new Chart(dailyEl, {
                type: 'bar',
                data: {
                    labels: payload.diario.labels,
                    datasets: [
                        {
                            label: 'Receitas',
                            data: payload.diario.receitas,
                            backgroundColor: 'rgba(52, 211, 153, 0.7)',
                            borderColor: 'rgba(52, 211, 153, 1)',
                            borderWidth: 1,
                            borderRadius: 4
                        },
                        {
                            label: 'Despesas',
                            data: payload.diario.despesas,
                            backgroundColor: 'rgba(248, 113, 113, 0.7)',
                            borderColor: 'rgba(248, 113, 113, 1)',
                            borderWidth: 1,
                            borderRadius: 4
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: {
                            grid: { display: false },
                            title: { display: true, text: 'Dia do mês', color: 'rgba(148, 163, 184, 0.9)' }
                        },
                        y: {
                            grid: { color: 'rgba(65, 139, 224, 0.08)' },
                            ticks: {
                                callback: function (v) { return currency.format(v); }
                            }
                        }
                    },
                    plugins: {
                        legend: {
                            position: 'top',
                            align: 'end',
                            labels: { boxWidth: 12, padding: 16 }
                        },
                        tooltip: {
                            callbacks: {
                                label: function (ctx) {
                                    return ' ' + ctx.dataset.label + ': ' + currency.format(ctx.parsed.y);
                                }
                            }
                        }
                    }
                }
            });
        }
    }

    function doughnutOptions() {
        return {
            responsive: true,
            maintainAspectRatio: true,
            cutout: '62%',
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: { boxWidth: 12, padding: 14, font: { size: 11 } }
                },
                tooltip: {
                    callbacks: {
                        label: function (ctx) {
                            var total = ctx.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                            var pct = total > 0 ? ((ctx.parsed / total) * 100).toFixed(1) : 0;
                            return ' ' + ctx.label + ': ' + currency.format(ctx.parsed) + ' (' + pct + '%)';
                        }
                    }
                }
            }
        };
    }

    function horizontalBarOptions() {
        return {
            indexAxis: 'y',
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                x: {
                    grid: { color: 'rgba(65, 139, 224, 0.08)' },
                    ticks: { callback: function (v) { return currency.format(v); } }
                },
                y: { grid: { display: false } }
            },
            plugins: {
                legend: {
                    position: 'top',
                    align: 'end',
                    labels: { boxWidth: 12, padding: 16 }
                },
                tooltip: {
                    callbacks: {
                        label: function (ctx) {
                            return ' ' + ctx.dataset.label + ': ' + currency.format(ctx.parsed.x);
                        }
                    }
                }
            }
        };
    }
})();
