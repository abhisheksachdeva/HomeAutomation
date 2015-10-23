from django.conf.urls import include, url
from . import views

urlpatterns = [

    url(r'^$', views.index, name='home'),

    # ex: /automate/5/
    #url(r'^(?P<request_id>[0-9]+)/$', views.detail, name='detail'),
]
