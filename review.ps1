echo "================" > review.txt
echo "shoppingcart-api" >> review.txt
echo "================" >> review.txt
git --no-pager log master.. --stat --pretty -- shoppingcart-api >> review.txt
echo "----------------" >> review.txt
echo "" >> review.txt


echo "================" >> review.txt
echo "shoppingcart-web" >> review.txt
echo "================" >> review.txt
git --no-pager log master.. --stat --pretty -- shoppingcart-web >> review.txt
echo "----------------" >> review.txt
echo "" >> review.txt

echo "================" >> review.txt
echo "identityServer6" >> review.txt
echo "================" >> review.txt
git --no-pager log master.. --stat --pretty -- identityServer6 >> review.txt
echo "----------------" >> review.txt
echo "" >> review.txt
