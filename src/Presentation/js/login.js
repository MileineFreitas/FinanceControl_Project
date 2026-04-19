document.getElementById('btn-login').addEventListener('click', async () =>{
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    const loginRequest = {
        email: email,
        password: password
    };

    try{
        const response = await fetch('https://localhost:7189/user/login',{
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginRequest)
        });

        if(response.ok){
            const data = await response.json();

            localStorage.setItem('userId', data.userId);
            localStorage.setItem('userName', data.name);
            localStorage.setItem('userEmail', data.email);

            window.location.href = '/home';
        }
        else if(response.status == 401){
            alert('Dados invalidos!');
        }
        else{
            alert('Erro inesperado...');
        }
        
    }catch(error){
        console.error('Erro de comunicação com o servidor....', error);
        alert('Erro de conexão com servidor');
    }
});