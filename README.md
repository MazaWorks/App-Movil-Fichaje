![QindelBot | Bot for Rocket.Chat Qindel stuff](https://pbs.twimg.com/profile_images/715503070709460992/sAD8q8Ay_400x400.jpg)

# App Fichaje Qindel
# App movil para fichar a traves de la API de Kimai

## Como usar la App
Esta app permite fichar en la api kimai especifica de Qindel, pero se puede modificar para peticiones REST de cualquier otra aplicación. 

Al iniciar la aplicación, se muestra una vista como la siguiente:

![Main View](https://github.com/MazaWorks/FichajeQindel/blob/master/Images/MainView.jpeg)

Basicamente tenemos 3 opciones Fichar la entrada(Llegue), Fichar la salida(Me Voy), y refrescar el contador, que manda una petición a la API kimai para ver si fichaste o no y cuanto tiempo llevas fichado (Refresca el contador por si has modificado algo por otro medio que no sea el de esta app)

Para que esta app funcione bien se necesita acceso a la red, y tus datos para que la petición sea aceptada por la API. Si hay un error en una de estas dos condiciones, se bloquearán las opciones de fichar entrada y salida y saldrá una vista como esta:

![Error View](https://github.com/MazaWorks/FichajeQindel/blob/master/Images/ErrorView.jpeg)

En este caso, los datos que se necesitan son tu username y tu API-token, para guardarlos en Aplication.Properties debes pulsar el icono de la parte superior derecha y te saldrá una vista como esta:

![Options View](https://github.com/MazaWorks/FichajeQindel/blob/master/Images/OptionsView.jpeg)

Aquí introducimos los distintos campos requeridos, asi como el tiempo que quieres que pase antes de que se envíe una notificación recordándote que tienes que dejar de fichar.