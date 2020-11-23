class CustomerModel {
  String id;
  String name;
  String description;
  String note;
  String customerGroupName;
  List<dynamic> contacts;

  CustomerModel({this.id, this.name, this.description, this.note, this.customerGroupName, this.contacts});

  factory CustomerModel.fromJson(Map<String, dynamic> json) {
    return CustomerModel(
        id: json['Id'] as String,
        name: json['Name'] as String,
        description: json['Description'] as String,
        note: json['Note'] as String,
        customerGroupName: json['CustomerGroupName'] as String,
        contacts: json['Contacts'] as List<dynamic>
    );
  }
}