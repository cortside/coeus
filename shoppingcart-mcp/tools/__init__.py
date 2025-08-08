from app.mcp_instance import mcp
from tools.get_api_health import get_api_health  # registers via decorator
from tools.get_api_settings import get_api_settings  # registers via decorator
from tools.get_api_v1_authorization import get_api_v1_authorization  # registers via decorator
from tools.get_api_v1_customers import get_api_v1_customers  # registers via decorator
from tools.post_api_v1_customers import post_api_v1_customers  # registers via decorator
from tools.get_api_v1_customers__id import get_api_v1_customers__id  # registers via decorator
from tools.put_api_v1_customers__id import put_api_v1_customers__id  # registers via decorator
from tools.post_api_v1_customers__resourceId__orders import post_api_v1_customers__resourceId__orders  # registers via decorator
from tools.post_api_v1_customers__resourceId__publish import post_api_v1_customers__resourceId__publish  # registers via decorator
from tools.post_api_v1_customers_search import post_api_v1_customers_search  # registers via decorator
from tools.get_api_v1_customers_search import get_api_v1_customers_search  # registers via decorator
from tools.get_api_v1_orders import get_api_v1_orders  # registers via decorator
from tools.post_api_v1_orders import post_api_v1_orders  # registers via decorator
from tools.get_api_v1_orders__id import get_api_v1_orders__id  # registers via decorator
from tools.put_api_v1_orders__id import put_api_v1_orders__id  # registers via decorator
from tools.post_api_v1_orders__id__items import post_api_v1_orders__id__items  # registers via decorator
from tools.post_api_v1_orders__resourceId__publish import post_api_v1_orders__resourceId__publish  # registers via decorator