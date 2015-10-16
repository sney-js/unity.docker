provider "aws" {
    access_key = "AKIAIQBIT634TZHJ4XIA"
    secret_key = "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"
    region = "eu-west-1"
}

resource "aws_instance" "space-docker-web" {
      
    connection {
      user = "ubuntu"
      key_file = "snehil-devops2.pem"
    }

    key_name = "snehil-devops2"
    ami = "ami-47a23a30"
    instance_type = "t2.micro"
    security_groups = ["launch-wizard-6"]

    

    provisioner "remote-exec" {
      inline = [
        "mkdir public_html",
        "sudo apt-get -y update",
        "sudo apt-get -y install nodejs nodejs-dev npm",
        "./public_html/runweb.sh"  
      ]
    }

    provisioner "file" {
        source = "public_html/"
        destination = "/home/ubuntu/public_html"
    }



}