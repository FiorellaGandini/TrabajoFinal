document.addEventListener('DOMContentLoaded', () => {
    
    // Usé una imagen neutra genérica
    const neutralImage = "https://placehold.co/600x600/e6f2ff/004080?text=FOTO+EVENTO";

    const mockEvents = [
        { id: 1, nombre: "La Cena de los Tontos", categoria: "Teatro", fecha: "06/09/2026", imagen: neutralImage },
        { id: 2, nombre: "Partido Liga Final", categoria: "Deporte", fecha: "15/11/2026", imagen: neutralImage },
        { id: 3, nombre: "Recital de Rock", categoria: "Concierto", fecha: "20/10/2026", imagen: neutralImage },
        { id: 4, nombre: "Pirulo en el Aire", categoria: "Stand-up", fecha: "04/09/2026", imagen: neutralImage },
        { id: 5, nombre: "Tech Summit 2026", categoria: "Conferencia", fecha: "05/12/2026", imagen: neutralImage },
        { id: 6, nombre: "Santa Fe en la Casa #1", categoria: "Fiesta", fecha: "03/09/2026", imagen: neutralImage },
        { id: 7, nombre: "La Bella Durmiente", categoria: "Teatro", fecha: "04/09/2026", imagen: neutralImage }
    ];

    let currentCategory = "Todos";
    let searchText = "";
    
    const eventGrid = document.getElementById('eventGrid');
    const searchInput = document.getElementById('searchInput');
    const categoryBtns = document.querySelectorAll('.cat-btn');

    function renderEvents(events) {
        eventGrid.innerHTML = ''; 

        if (events.length === 0) {
            eventGrid.innerHTML = '<p class="no-results">No se encontraron eventos.</p>';
            return;
        }

        events.forEach(evento => {
            const card = document.createElement('div');
            card.className = 'event-card';
            // Al hacer clic en la tarjeta entera, te manda a comprar
            card.onclick = () => alert(`Redirigiendo a la compra de: ${evento.nombre}`);
            card.innerHTML = `
                <img src="${evento.imagen}" alt="Evento" class="event-img">
                <div class="card-overlay">
                    <div class="card-title">${evento.nombre}</div>
                    <div class="card-date">📅 ${evento.fecha}</div>
                </div>
            `;
            eventGrid.appendChild(card);
        });
    }

    function updateView() {
        let filtered = mockEvents.filter(e => currentCategory === "Todos" || e.categoria === currentCategory);
        
        if (searchText.trim() !== "") {
            filtered = filtered.filter(e => e.nombre.toLowerCase().includes(searchText.toLowerCase()));
        }

        renderEvents(filtered);
    }

    searchInput.addEventListener('input', (e) => {
        searchText = e.target.value;
        updateView();
    });

    categoryBtns.forEach(btn => {
        btn.addEventListener('click', (e) => {
            categoryBtns.forEach(b => b.classList.remove('active'));
            e.currentTarget.classList.add('active');
            
            currentCategory = e.currentTarget.dataset.cat;
            updateView();
        });
    });

    updateView();
});