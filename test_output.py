from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS, ASYNCHRONOUS
from datetime import datetime, timedelta
from time import sleep
import pandas as pd
import random

client = InfluxDBClient(url="http://localhost:8086", token="CJqT91ukeq4byDi0FreNlWz8CjO6lvt26PIM_l0okpJxk_-lTMSg9h1n92SaflMNwvrz9prh4lCuWQz_6Fk5Dg==", org="organization")
writer = client.write_api(write_options=SYNCHRONOUS)
while True:
    point = Point("modbus")
    point.time(datetime.now())
    point.tag("host", "simulator")
    point.tag("sensor_name", "light_sensor")
    point.field("value", random.uniform(0, 1000))
    print(point)
    writer.write("temp_diff", record=point)
    sleep(5)
