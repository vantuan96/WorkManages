import 'package:flutter/material.dart';

typedef DiaryEditPage_Value = void Function(String title, String description);

class DiaryEditPage extends StatelessWidget {
  final DiaryEditPage_Value onSave;
  final String buttonTitle;
  final String title;
  final String description;

  DiaryEditPage({this.buttonTitle, this.onSave, this.title, this.description});

  TextEditingController controller_title = new TextEditingController();
  TextEditingController controller_description = new TextEditingController();

  @override
  Widget build(BuildContext context) {
    controller_title.text = title;
    controller_description.text = description;

    return Container(
      color: Color(0xff757575),
      child: Container(
        padding: EdgeInsets.all(20.0),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.only(
            topLeft: Radius.circular(20.0),
            topRight: Radius.circular(20.0),
          ),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: <Widget>[
            Text(
              'Title',
              textAlign: TextAlign.start,
              style: TextStyle(
                fontSize: 30.0,
                color: Colors.lightBlueAccent,
              ),
            ),
            TextField(
              autofocus: true,
              textAlign: TextAlign.start,
              controller: controller_title,
            ),
            Text(
              'Description',
              textAlign: TextAlign.start,
              style: TextStyle(
                fontSize: 30.0,
                color: Colors.lightBlueAccent,
              ),
            ),
            TextField(
              autofocus: false,
              maxLines: 3,
              textAlign: TextAlign.start,
              controller: controller_description,
            ),
            FlatButton(
              child: Text(
                buttonTitle,
                style: TextStyle(
                  color: Colors.white,
                ),
              ),
              color: Colors.lightBlueAccent,
              onPressed: () {
                onSave(controller_title.text, controller_description.text);


                Navigator.pop(context);
              },
            ),
          ],
        ),
      ),
    );
  }
}
