# Небольшое пояснение

## Изменения в модели user
Было принято решение сделать атрибуты ModifiedOn, ModifiedBy, RevokedOn, RevokedBy nullable так как пользователя никто не изменял и никто не удалял, то эти артибуты не должны быть обозначены, что явно даёт увидеть отсутсвие изменений. А также модель User не содержит пароль, а его хэш.

## Изменения принципа работы UPDATE методов
Вместо нескольких отдельны методов было принято решение организовать один (два, если считать админский), который принимает в себя dto. Обозначенные поля буду обновлены, а установленные в значение null или необозначенные останутся неизменённымми.

## Принцип работы
Была реализована генерация токена и работа с ним. У админов есть свои уникальные методы, но, в то же время, они могут использовать и методы обычного пользователя. Пользователь не может запускать методы админа. Информация о том, является ли пользователем админом, находится в токене. Как раз из-за использования токенов запросы не содержать логин и пароль отправляющего запрос.

Также для регистрации обычных пользователей и их логина в систему был сделан отдельный контроллер AuthenticationController.

Всё остальное было выполнено в соотвествии с техническим заданием.

## Ps
Данные default админа:

+ Логин - adminadmin
+ Пароль - admin_11
