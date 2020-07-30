# ToDo Tasks Manager
##### This project keeps all of your current tasks and implements basic CRUD functionality. 
User authentication is handled by the ASP .NET Core **Identity** class. You can enable the "Remember Me" feature, and if you re-open the login page within 30 minutes or so, you will be redirected to your To-Do list.  All tasks are stored in the **MS SQL Server localdb** and are associated with a specific user that created them, so on login you can see and edit only those tasks that are created by you.  Form validation is done on the client side when possible using built-in **jquery.validate.unobtrusive.js** functionality, so the user can't post an invalid form. 
#### Tech Stack
- Bootstrap 4
- JQuery
- ASP.NET Core MVC
- Entity Framework
#### A Few Screenshots
This project was developed only with educational purpose in mind, so it is not hosted anywhere. To illustrate it's functionality, I've added a couple of screenshots.


![Home Page](https://github.com/cryingwhitecat/ToDo/blob/imgs/imgs/index-page.png)

![Login Page](https://github.com/cryingwhitecat/ToDo/blob/imgs/imgs/login-page.png)

![Register Page](https://github.com/cryingwhitecat/ToDo/blob/imgs/imgs/register-page.png)

![Tasks List](https://github.com/cryingwhitecat/ToDo/blob/imgs/imgs/todo-list.png)

![Edit Item Form](https://github.com/cryingwhitecat/ToDo/blob/imgs/imgs/edit-form.png)
